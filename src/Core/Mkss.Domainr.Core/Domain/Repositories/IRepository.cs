using System.Threading;
using System.Threading.Tasks;
using Domainr.Core.Domain.Model;

namespace Domainr.Core.Domain.Repositories
{
    public interface IRepository<TAggregateRoot, in TAggregateRootId>
        where TAggregateRoot : IAggregateRoot<TAggregateRootId>
        where TAggregateRootId : class, IAggregateRootId
    {
        Task<TAggregateRoot> GetByIdAsync(string serializedId, CancellationToken cancellationToken = default);

        Task SaveAsync(TAggregateRoot aggregateRoot, string metadata = default, CancellationToken cancellationToken = default);
    }
}