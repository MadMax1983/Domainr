using Domainr.Core.Domain.Model;

namespace Domainr.Core.Tests.TestDoubles
{
    internal interface ITestAggregateRoot
        : IAggregateRoot<TestAggregateRootId>
    {
        public void ExecuteBehavior();
        
        public void ExecuteUnhandledBehavior();
    }
}