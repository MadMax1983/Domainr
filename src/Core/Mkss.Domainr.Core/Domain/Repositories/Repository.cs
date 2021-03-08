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
        where TAggregateRootId : class, IAggregateRootId
    {
        protected Repository(IEventStore eventStore)
        {
            EventStore = eventStore;
        }

        protected IEventStore EventStore { get; }

        public virtual async Task<TAggregateRoot> GetByIdAsync(TAggregateRootId aggregateRootId, CancellationToken cancellationToken = default)
        {
            var eventStream = (await EventStore.GetByAggregateRootIdAsync(aggregateRootId.ToString(), Constants.INITIAL_VERSION, cancellationToken)).ToList();
            if (!eventStream.Any())
            {
                return null;
            }

            var aggregateRoot = new TAggregateRoot();

            aggregateRoot.LoadFromStream(eventStream);

            return aggregateRoot;
        }

        public virtual async Task SaveAsync(TAggregateRoot aggregateRoot, string metadata = default, CancellationToken cancellationToken = default)
        {
            if (aggregateRoot == null)
            {
                throw new AggregateRootNullException(nameof(aggregateRoot));
            }

            var uncommittedEvents = aggregateRoot.GetUncommittedChanges();
            if (!uncommittedEvents.Any())
            {
                throw new AggregateRootException(string.Format(ExceptionResources.NoEventsToCommit, aggregateRoot.Id, aggregateRoot.Version));
            }

            var committedEvents = aggregateRoot.CommitChanges(aggregateRoot.Version);

            await EventStore.SaveAsync(committedEvents, metadata, cancellationToken);
        }
    }
}