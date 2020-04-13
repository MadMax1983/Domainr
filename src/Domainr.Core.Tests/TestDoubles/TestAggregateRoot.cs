using Domainr.Core.Domain.Model;

namespace Domainr.Core.Tests.TestDoubles
{
    internal sealed class TestAggregateRoot
        : AggregateRoot<TestAggregateRootId>
    {

        public void InitializeId(string aggregateRootId, bool initializeAggregateRootId)
        {
            ApplyChange(new TestEvent1(aggregateRootId, initializeAggregateRootId));
        }

        public void ExecuteSomeAction()
        {
            ApplyChange(new TestEvent2(Id.ToString()));
        }

        private void On(TestEvent1 @event)
        {
            Id = @event.InitializeAggregateRootId
                ? new TestAggregateRootId(@event.AggregateRootId)
                : null;
        }

        private void On(TestEvent2 @event)
        {
        }
    }
}