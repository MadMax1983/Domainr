using Domainr.Core.Domain.Model;

namespace Domainr.Core.Domain.Factories
{
    public interface IFactory<out TAggregateRoot, TAggregateRootId>
        where TAggregateRoot : AggregateRoot<TAggregateRootId>, IAggregateRoot<TAggregateRootId>
        where TAggregateRootId : class, IAggregateRootId
    {
        TAggregateRoot Create();
    }
}