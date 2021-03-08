using Domainr.Core.Domain.Repositories;
using Domainr.Core.EventSourcing.Abstraction;

namespace Domainr.Core.Tests.TestDoubles
{
    internal sealed class TestRepository
        : Repository<TestAggregateRoot, TestAggregateRootId>
    {
        public TestRepository(IEventStore eventStore)
            : base(eventStore)
        {
        }
    }
}