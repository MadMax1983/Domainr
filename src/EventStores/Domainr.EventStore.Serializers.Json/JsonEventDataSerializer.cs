using System;
using Domainr.Core.EventSourcing.Abstraction;
using Newtonsoft.Json;

namespace Domainr.EventStore.Serializers.Json
{
    public class JsonEventDataSerializer
        : IEventDataSerializer<string>
    {
        public virtual string Serialize<TEvent>(TEvent @event)
            where TEvent : Event
        {
            var jsonString = JsonConvert.SerializeObject(@event);

            return jsonString;
        }

        public Event Deserialize(string jsonString, string type)
        {
            var eventType = Type.GetType(type);
            if (eventType == null)
            {
                throw new NullReferenceException(nameof(eventType));
            }

            var settings = new JsonSerializerSettings();

            var @event = (Event) JsonConvert.DeserializeObject(jsonString, eventType, settings);

            return @event;
        }
    }
}