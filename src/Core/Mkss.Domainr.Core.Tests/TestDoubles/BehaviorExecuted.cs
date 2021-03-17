using Domainr.Core.EventSourcing.Abstraction;

namespace Domainr.Core.Tests.TestDoubles
{
    public sealed class BehaviorExecuted
        : Event
    {
        public BehaviorExecuted(string aggregateRootId)
            : base(aggregateRootId)
        {
        }
    }
}