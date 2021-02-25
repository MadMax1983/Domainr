using Domainr.Core.Domain.Model;

namespace Domainr.Core.Tests.TestDoubles
{
    internal sealed class TestAggregateRoot
        : AggregateRoot<TestAggregateRootId>
    {
        public TestAggregateRoot()
        {
        }
        
        public TestAggregateRoot(string serializedAggregateRootId)
            : base(new TestAggregateRootId(serializedAggregateRootId))
        {
        }
        
        protected override TestAggregateRootId CreateAggregateRootId(string serializedAggregateRootId)
        {
            return new TestAggregateRootId(serializedAggregateRootId);
        }

        public void ExecuteSomeAction()
        {
            ApplyChange(new TestEvent2(Id.ToString()));
        }

        public void DoSomethingUnhandled()
        {
            ApplyChange(new TestEvent3(Id.ToString()));
        }

        public void ExecuteAnotherAction()
        {
            ApplyChange(new TestEvent4(Id.ToString()));
        }

        private void On(TestEvent2 @event)
        {
        }

        private void On(TestEvent4 @event)
        {
        }
    }
}