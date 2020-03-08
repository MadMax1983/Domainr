using Domainr.Core.EventSourcing.Abstraction;

namespace Domainr.Core.Tests.TestDoubles
{
    internal sealed class TestEvent
        : Event
    {
        /// <inheritdoc/>
        public TestEvent()
        {
        }

        /// <inheritdoc/>
        public TestEvent(string aggregateRootId, bool initializeAggregateRootId)
            : base(aggregateRootId)
        {
            InitializeAggregateRootId = initializeAggregateRootId;
        }

        public bool InitializeAggregateRootId { get; }
    }
}