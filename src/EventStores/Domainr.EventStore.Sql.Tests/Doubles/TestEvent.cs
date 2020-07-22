using System;
using System.Reflection;
using Domainr.Core.EventSourcing.Abstraction;
using Newtonsoft.Json;

namespace Domainr.EventStore.Sql.Tests.Doubles
{
    [Serializable]
    internal sealed class TestEvent
        : Event
    {
        public TestEvent()
        {
        }

        [JsonConstructor]
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