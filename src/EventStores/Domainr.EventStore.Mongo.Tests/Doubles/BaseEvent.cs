using System.Reflection;
using Domainr.Core.EventSourcing.Abstraction;

namespace Domainr.EventStore.Mongo.Tests.Doubles
{
    internal abstract class BaseEvent
        : Event
    {
        protected BaseEvent()
        {
        }

        protected BaseEvent(string aggregateRootId)
            : base(aggregateRootId)
        {
        }

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