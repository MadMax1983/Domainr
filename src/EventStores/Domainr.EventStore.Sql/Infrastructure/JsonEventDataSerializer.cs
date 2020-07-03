using System;
using Domainr.Core.EventSourcing.Abstraction;
using Newtonsoft.Json;

namespace Domainr.EventStore.Sql.Infrastructure
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
            var @event = (Event) JsonConvert.DeserializeObject(jsonString, Type.GetType(type));

            return @event;
        }
    }
}