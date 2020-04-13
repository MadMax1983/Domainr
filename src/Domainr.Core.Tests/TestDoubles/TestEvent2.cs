using Domainr.Core.EventSourcing.Abstraction;

namespace Domainr.Core.Tests.TestDoubles
{
    internal sealed class TestEvent2
        : Event
    {
        /// <inheritdoc/>
        public TestEvent2(string aggregateRootId)
            : base(aggregateRootId)
        {
        }
    }
}