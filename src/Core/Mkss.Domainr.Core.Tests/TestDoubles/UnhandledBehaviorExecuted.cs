using Domainr.Core.EventSourcing.Abstraction;

namespace Domainr.Core.Tests.TestDoubles
{
    internal sealed class UnhandledBehaviorExecuted
        : Event
    {
        public UnhandledBehaviorExecuted(string aggregateRootId)
            : base(aggregateRootId)
        {
        }
    }
}