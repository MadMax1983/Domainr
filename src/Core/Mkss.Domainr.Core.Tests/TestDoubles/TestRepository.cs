using Domainr.Core.Domain.Factories;
using Domainr.Core.Domain.Repositories;
using Domainr.Core.EventSourcing.Abstraction;

namespace Domainr.Core.Tests.TestDoubles
{
    internal sealed class TestRepository
        : Repository<ITestAggregateRoot, TestAggregateRootId>
    {
        public TestRepository(IAggregateRootFactory<ITestAggregateRoot, TestAggregateRootId> factory, IEventStore eventStore)
            : base(factory, eventStore)
        {
        }
    }
}