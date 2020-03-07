using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Domainr.Core.EventSourcing.Abstraction;
using Domainr.Core.Exceptions;
using Domainr.Core.Infrastructure;
using Domainr.Core.Resources;

namespace Domainr.Core.Domain.Model
{
    public abstract class AggregateRoot<TAggregateRootId>
        where TAggregateRootId : IAggregateRootId
    {
        private const string ON_METHOD_NAME = "On";

        private readonly List<Event> _changes;

        private List<MethodInfo> _onMethods;

        private TAggregateRootId _aggregateRootId;

        protected AggregateRoot()
        {
            var onMethods = GetOnMethodsFromType();

            SetOnMethods(onMethods);

            _changes = new List<Event>();

            Version = Constants.INITIAL_VERSION;
        }

        public TAggregateRootId Id
        {
            get => _aggregateRootId;

            protected set
            {
                if (_aggregateRootId != null)
                {
                    throw new AggregateRootIdException(ExceptionResources.CannotChangeAggregateRootId);
                }

                _aggregateRootId = value;
            }
        }

        public long Version { get; private set; }

        internal IReadOnlyCollection<Event> GetUncommittedChanges()
        {
            return _changes.ToList();
        }

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

            Version = lastEvent.Version;
        }

        internal IReadOnlyCollection<Event> CommitChanges(long currentAggregateRootVersion)
        {
            if (!_changes.Any())
            {
                return new Event[0];
            }

            var version = SetCurrentVersion(currentAggregateRootVersion);

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

        private void ValidateEventStream(IReadOnlyCollection<Event> eventStream)
        {
            if (eventStream == null)
            {
                throw new EventStreamNullException(nameof(eventStream));
            }

            if (!eventStream.Any())
            {
                throw new EmptyEventStreamException(ExceptionResources.EmptyAggregateRootEventStream);
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
            var onMethod = _onMethods.Single(hm => hm.GetParameters().Single().ParameterType == @event.GetType());

            onMethod.Invoke(this, new object[] { @event });
        }

        private long SetCurrentVersion(long currentAggregateRootVersion)
        {
            AggregateRootVersionValidator.Validate(currentAggregateRootVersion);

            return Version < currentAggregateRootVersion
                ? currentAggregateRootVersion
                : Version;
        }
    }
}