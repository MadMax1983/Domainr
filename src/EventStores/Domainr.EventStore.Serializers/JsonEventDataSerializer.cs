using System;
using System.Reflection;
using Domainr.Core.EventSourcing.Abstraction;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Domainr.EventStore.Serializers
{
    public class JsonEventDataSerializer
        : IEventDataSerializer<string>
    {
        public virtual string Serialize<TEvent>(TEvent @event)
            where TEvent : Event
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            var jsonString = JsonConvert.SerializeObject(@event);

            return jsonString;
        }

        public Event Deserialize(string jsonString, string type)
        {
            var eventType = Type.GetType(type);
            if (eventType == null)
            {
                throw new InvalidOperationException("Provided type is incorrect");
            }

            var jObj = JObject.Parse(jsonString);

            var version = jObj.GetValue("Version").Value<long>();

            var @event = (Event)jObj.ToObject(eventType);

            var versionField = typeof(Event).GetField("_version", BindingFlags.Instance | BindingFlags.NonPublic);
            if (versionField != null)
            {
                versionField.SetValue(@event, version);
            }

            return @event;
        }
    }
}