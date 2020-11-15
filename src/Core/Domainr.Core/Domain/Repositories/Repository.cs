using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domainr.Core.Domain.Model;
using Domainr.Core.EventSourcing.Abstraction;
using Domainr.Core.Exceptions;
using Domainr.Core.Infrastructure;
using Domainr.Core.Resources;

namespace Domainr.Core.Domain.Repositories
{
    public abstract class Repository<TAggregateRoot, TAggregateRootId>
        : IRepository<TAggregateRoot, TAggregateRootId>
        where TAggregateRoot : AggregateRoot<TAggregateRootId>, new()
        where TAggregateRootId : IAggregateRootId
    {
        private readonly IEventStore _eventStore;

        protected Repository(IConcurrencyResolver concurrencyResolver, IEventStore eventStore)
        {
            ConcurrencyResolver = concurrencyResolver;

            _eventStore = eventStore;
        }

        protected IConcurrencyResolver ConcurrencyResolver { get; }

        public async Task<TAggregateRoot> GetByIdAsync(TAggregateRootId aggregateRootId, CancellationToken cancellationToken = default)
        {
            var eventStream = (await _eventStore.GetByAggregateRootIdAsync(aggregateRootId.ToString(), Constants.INITIAL_VERSION, cancellationToken)).ToList();
            if (!eventStream.Any())
            {
                return null;
            }

            var aggregateRoot = new TAggregateRoot();

            aggregateRoot.LoadFromStream(eventStream);

            return aggregateRoot;
        }

        public async Task SaveAsync(TAggregateRoot aggregateRoot, long expectedVersion, CancellationToken cancellationToken = default)
        {
            if (aggregateRoot == null)
            {
                throw new AggregateRootNullException(nameof(aggregateRoot));
            }

            var uncommittedEvents = aggregateRoot.GetUncommittedChanges();
            if (!uncommittedEvents.Any())
            {
                throw new AggregateRootException(string.Format(ExceptionResources.NoEventsToCommit, aggregateRoot.Id.ToString(), aggregateRoot.Version));
            }

            var aggregateRootVersion = aggregateRoot.Version;

            var concurrentEvents = await GetConcurrentEventsAsync(aggregateRoot.Id, expectedVersion, cancellationToken);
            if (concurrentEvents.Any())
            {
                CheckConcurrency(concurrentEvents, uncommittedEvents, aggregateRoot.Id, aggregateRoot.Version);

                aggregateRootVersion = concurrentEvents.Max(pe => pe.Version);
            }

            var committedEvents = aggregateRoot.CommitChanges(aggregateRootVersion);

            await _eventStore.SaveAsync(committedEvents, cancellationToken);
        }

        private async Task<IReadOnlyCollection<Event>> GetConcurrentEventsAsync(TAggregateRootId aggregateRootId, long expectedVersion, CancellationToken cancellationToken)
        {
            return expectedVersion <= Constants.INITIAL_VERSION
                ? new Event[0]
                : await _eventStore.GetByAggregateRootIdAsync(aggregateRootId.ToString(), expectedVersion, cancellationToken);
        }

        private void CheckConcurrency(IEnumerable<Event> concurrentEvents, IEnumerable<Event> uncommittedEvents, TAggregateRootId aggregateRootId, long aggregateRootVersion)
        {
            var concurrentEventTypes = concurrentEvents.Select(pe => pe.GetType()).ToList();

            if (uncommittedEvents.Any(uncommittedEvent => ConcurrencyResolver.ConflictsWith(uncommittedEvent.GetType(), concurrentEventTypes)))
            {
                throw new ConcurrencyException(string.Format(ExceptionResources.AggregateConcurrencyFound, aggregateRootId.ToString(), aggregateRootVersion));
            }
        }
    }
}