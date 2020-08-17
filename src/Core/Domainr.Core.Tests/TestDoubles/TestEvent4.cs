using Domainr.Core.EventSourcing.Abstraction;

namespace Domainr.Core.Tests.TestDoubles
{
    internal sealed class TestEvent4
        : Event
    {
        public TestEvent4(string aggregateRootId)
            : base(aggregateRootId)
        {
        }
    }
}