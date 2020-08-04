using System.Reflection;
using Domainr.Core.EventSourcing.Abstraction;

namespace Domainr.EventStore.InMemory.Tests.Doubles
{
    internal sealed class TestEvent
        : Event
    {
        public TestEvent(string aggregateRootId, string stringProp)
            : base(aggregateRootId)
        {
            StringProp = stringProp;
        }

        public string StringProp { get; }

        public void SetVersion(long version)
        {
            var versionField = typeof(Event).GetField("_version", BindingFlags.Instance | BindingFlags.NonPublic);
            if (versionField != null)
            {
                versionField.SetValue(this, version);
            }
        }
    }
}