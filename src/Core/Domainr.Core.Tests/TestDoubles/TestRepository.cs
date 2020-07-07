using Domainr.Core.Domain.Repositories;
using Domainr.Core.EventSourcing.Abstraction;

namespace Domainr.Core.Tests.TestDoubles
{
    internal sealed class TestRepository
        : Repository<TestAggregateRoot, TestAggregateRootId>
    {
        public TestRepository(IConcurrencyResolver concurrencyResolver, IEventStore eventStore, IEventPublisher eventPublisher)
            : base(concurrencyResolver, eventStore, eventPublisher)
        {
        }
    }
}