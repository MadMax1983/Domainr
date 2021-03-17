using Domainr.Core.Domain.Model;

namespace Domainr.Core.Domain.Factories
{
    public interface IAggregateRootFactory<out TAggregateRoot, TAggregateRootId>
        where TAggregateRoot : IAggregateRoot<TAggregateRootId>
        where TAggregateRootId : class, IAggregateRootId
    {
        TAggregateRoot Create();
    }
}