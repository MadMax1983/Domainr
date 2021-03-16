using Domainr.Core.Domain.Model;

namespace Domainr.Core.Tests.TestDoubles
{
    internal sealed class TestAggregateRoot
        : AggregateRoot<TestAggregateRootId>
    {
        public TestAggregateRoot()
        {
        }
        
        public static TestAggregateRoot Create()
        {
            var testAggregateRoot = new TestAggregateRoot();
            
            testAggregateRoot.ApplyChange(null);

            return testAggregateRoot;
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
            Id = new TestAggregateRootId("123");
        }

        private void On(TestEvent4 @event)
        {
        }

        protected override TestAggregateRootId RestoreIdFromString(string serializedId)
        {
            return new TestAggregateRootId(serializedId);
        }
    }
}