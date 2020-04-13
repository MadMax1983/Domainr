using Domainr.Core.EventSourcing.Abstraction;

namespace Domainr.Core.Tests.TestDoubles
{
    internal sealed class TestEvent1
        : Event
    {
        /// <inheritdoc/>
        public TestEvent1()
        {
        }

        /// <inheritdoc/>
        public TestEvent1(string aggregateRootId, bool initializeAggregateRootId)
            : base(aggregateRootId)
        {
            InitializeAggregateRootId = initializeAggregateRootId;
        }

        public bool InitializeAggregateRootId { get; }
    }
}