using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Domainr.Core.EventSourcing.Abstraction;
using Domainr.Core.Exceptions;
using Domainr.Core.Infrastructure;

namespace Domainr.Core.Domain.Model
{
    public abstract class AggregateRoot<TId>
        : IAggregateRoot<TId> where TId : class, IAggregateRootId
    {
        private const string ON_METHOD_NAME = "On";

        private readonly List<Event> _changes;

        private List<MethodInfo> _onMethods;

        private TId _id;

        protected AggregateRoot()
        {
            var onMethods = GetOnMethodsFromType();

            SetOnMethods(onMethods);

            _changes = new List<Event>();

            Version = Constants.INITIAL_VERSION;
        }

        protected AggregateRoot(TId id)
            : this()
        {
            Id = id;
        }

        public TId Id
        {
            get => _id;
            protected set
            {
                if (_id != null)
                {
                    throw new AggregateRootIdException("Aggregate root identifier has been already initialized.");
                }

                _id = value;
            }
        }

        public long Version { get; private set; }

        /// <summary>
        /// Restores an aggregate root state from an event stream.
        /// </summary>
        /// <param name="eventStream">An aggregate root event stream.</param>
        internal void LoadFromStream(IReadOnlyCollection<Event> eventStream)
        {
            ValidateEventStream(eventStream);

            var orderedEventStream = OrderEventStream(eventStream);

            ApplyEventStream(orderedEventStream);

            var lastEvent = orderedEventStream.Last();

            Id = RestoreIdFromString(lastEvent.AggregateRootId);
            Version = lastEvent.Version;
        }

        internal IReadOnlyCollection<Event> CommitChanges()
        {
            if (_id == null)
            {
                throw new AggregateRootIdException("Aggregate root identifier has not been initialized.");
            }

            if (!_changes.Any())
            {
                throw new AggregateRootException($"No changes were found for the aggregate root. Id: {Id}, Version: {Version}");
            }

            var version = Version;

            var changes = _changes.ToList();

            changes.ForEach(e => e.IncrementVersion(ref version));

            Version = version;

            _changes.Clear();

            return changes;
        }

        protected void ApplyChange(Event @event)
        {
            if (@event == null)
            {
                throw new EventNullException(nameof(@event));
            }

            ApplyChange(@event, true);
        }

        protected abstract TId RestoreIdFromString(string serializedId);

        private static void ValidateEventStream(IReadOnlyCollection<Event> eventStream)
        {
            if (eventStream == null)
            {
                throw new EventStreamNullException(nameof(eventStream));
            }

            if (!eventStream.Any())
            {
                throw new EmptyEventStreamException("Aggregate root events stream is empty.");
            }
        }

        private IEnumerable<MethodInfo> GetOnMethodsFromType()
        {
            return GetType()
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(mi => mi.Name == ON_METHOD_NAME);
        }

        private void SetOnMethods(IEnumerable<MethodInfo> onMethods)
        {
            _onMethods = new List<MethodInfo>();

            _onMethods.AddRange(onMethods);
        }

        private static IReadOnlyCollection<Event> OrderEventStream(IEnumerable<Event> eventStream)
        {
            var orderedEventStream = eventStream.OrderBy(e => e.Version).ToList();

            return orderedEventStream;
        }

        private void ApplyEventStream(IEnumerable<Event> eventStream)
        {
            foreach (var @event in eventStream)
            {
                ApplyChange(@event, false);
            }
        }

        private void ApplyChange(Event @event, bool isNew)
        {
            InvokeOnMethod(@event);

            if (isNew)
            {
                _changes.Add(@event);
            }
        }

        private void InvokeOnMethod(Event @event)
        {
            var onMethod = _onMethods.SingleOrDefault(hm => hm.GetParameters().Single().ParameterType == @event.GetType());

            onMethod.Invoke(this, new object[] { @event });
        }
    }
}