using Domainr.Core.Domain.Model;

namespace Domainr.Core.Tests.TestDoubles
{
    internal sealed class TestAggregateRoot
        : AggregateRoot<TestAggregateRootId>, ITestAggregateRoot
    {
        public TestAggregateRoot()
        {
        }
        
        public TestAggregateRoot(TestAggregateRootId id)
            : base(id)
        {
        }

        public void SetId(string id)
        {
            Id = new TestAggregateRootId(id);
        }

        public void ExecuteBehavior()
        {
            ApplyChange(new BehaviorExecuted(Id.ToString()));
        }

        public void ExecuteUnhandledBehavior()
        {
            ApplyChange(new UnhandledBehaviorExecuted(Id.ToString()));
        }

        protected override TestAggregateRootId RestoreIdFromString(string serializedId)
        {
            return new TestAggregateRootId(serializedId);
        }

        private void On(BehaviorExecuted @event)
        {
        }
    }
}