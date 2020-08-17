using Domainr.Core.EventSourcing.Abstraction;

namespace Domainr.Core.Tests.TestDoubles
{
    internal sealed class TestEvent3
        : Event
    {
        public TestEvent3(string aggregateRootId)
            : base(aggregateRootId)
        {
        }
    }
}