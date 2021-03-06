﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domainr.Core.Domain.Model;
using Domainr.Core.EventSourcing.Abstraction;
using Domainr.Core.Exceptions;
using Domainr.Core.Infrastructure;

namespace Domainr.Core.Domain.Repositories
{
    public class Repository<TAggregateRoot, TAggregateRootId>
        : IRepository<TAggregateRoot, TAggregateRootId>
        where TAggregateRoot : AggregateRoot<TAggregateRootId>, IAggregateRoot<TAggregateRootId>, new()
        where TAggregateRootId : class, IAggregateRootId
    {
        public Repository(IEventStore eventStore)
        {
            EventStore = eventStore;
        }

        protected IEventStore EventStore { get; }

        public virtual async Task<TAggregateRoot> GetByIdAsync(string aggregateRootId, CancellationToken cancellationToken = default)
        {
            var eventStream = await EventStore.GetByAggregateRootIdAsync(aggregateRootId, Constants.INITIAL_VERSION, cancellationToken);
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

            var committedEvents = aggregateRoot.CommitChanges();

            await EventStore.SaveAsync(committedEvents, metadata, cancellationToken);
        }
    }
}