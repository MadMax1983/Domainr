using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domainr.Core.Domain.Factories;
using Domainr.Core.Domain.Model;
using Domainr.Core.EventSourcing.Abstraction;
using Domainr.Core.Exceptions;
using Domainr.Core.Infrastructure;

namespace Domainr.Core.Domain.Repositories
{
    public class Repository<TAggregateRoot, TAggregateRootId>
        : IRepository<TAggregateRoot, TAggregateRootId>
        where TAggregateRoot : IAggregateRoot<TAggregateRootId>
        where TAggregateRootId : class, IAggregateRootId
    {
        public Repository(IAggregateRootFactory<TAggregateRoot, TAggregateRootId> factory, IEventStore eventStore)
        {
            Factory = factory;
            
            EventStore = eventStore;
        }

        protected IAggregateRootFactory<TAggregateRoot, TAggregateRootId> Factory { get; }
        
        protected IEventStore EventStore { get; }

        public virtual async Task<TAggregateRoot> GetByIdAsync(string aggregateRootId, CancellationToken cancellationToken = default)
        {
            var eventStream = await EventStore.GetByAggregateRootIdAsync(aggregateRootId, Constants.INITIAL_VERSION, cancellationToken);
            if (!eventStream.Any())
            {
                return default;
            }

            var aggregateRoot = Factory.Create();

            aggregateRoot.LoadFromStream(eventStream);

            return aggregateRoot;
        }

        public virtual async Task SaveAsync(TAggregateRoot aggregateRoot, string metadata = default, CancellationToken cancellationToken = default)
        {
            if (aggregateRoot == null)
            {
                throw new AggregateRootNullException(nameof(aggregateRoot));
            }

            var changesToSave = aggregateRoot.GetUncommittedChanges();
            if (!changesToSave.Any())
            {
                return;
            }

            aggregateRoot.CommitChanges();

            await EventStore.SaveAsync(changesToSave, metadata, cancellationToken);
        }
    }
}