using Domainr.Core.Domain.Model;

namespace Domainr.Core.Tests.TestDoubles
{
    internal sealed class TestAggregateRoot
        : AggregateRoot<TestAggregateRootId>
    {

        public void InitializeId(string aggregateRootId, bool initializeAggregateRootId)
        {
            ApplyChange(new TestEvent(aggregateRootId, initializeAggregateRootId));
        }

        private void On(TestEvent @event)
        {
            Id = @event.InitializeAggregateRootId
                ? new TestAggregateRootId(@event.AggregateRootId)
                : null;
        }
    }
}