using Domainr.Core.EventSourcing.Abstraction;

namespace Domainr.Core.Tests.TestDoubles
{
    internal sealed class TestEvent4
        : Event
    {
        /// <inheritdoc/>
        public TestEvent4()
        {
        }

        /// <inheritdoc/>
        public TestEvent4(string aggregateRootId)
            : base(aggregateRootId)
        {
        }
    }
}