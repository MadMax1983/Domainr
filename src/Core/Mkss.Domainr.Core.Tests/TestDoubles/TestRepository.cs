using Domainr.Core.Domain.Factories;
using Domainr.Core.Domain.Repositories;
using Domainr.Core.EventSourcing.Abstraction;

namespace Domainr.Core.Tests.TestDoubles
{
    internal sealed class TestRepository
        : Repository<TestAggregateRoot, TestAggregateRootId>
    {
        public TestRepository(IFactory<TestAggregateRoot, TestAggregateRootId> aggregateRootFactory, IEventStore eventStore)
            : base(aggregateRootFactory, eventStore)
        {
        }
    }
}