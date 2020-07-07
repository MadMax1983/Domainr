using System;
using Domainr.Core.EventSourcing.Abstraction;

namespace Domainr.EventStore.Sql.Tests.Doubles
{
    [Serializable]
    internal sealed class TestEvent
        : Event
    {
        private readonly long _versionField = -1;

        public TestEvent()
        {
        }

        public TestEvent(string aggregateRootId)
            : base(aggregateRootId)
        {
        }

        public long VersionField => _versionField;
    }
}