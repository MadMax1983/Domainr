using Domainr.Core.EventSourcing.Abstraction;

namespace Domainr.EventStore.Sql.Infrastructure
{
    public interface IEventDataSerializer<TEventDataSerializationType>
    {
        TEventDataSerializationType Serialize<TEvent>(TEvent @event)
            where TEvent : Event;

        Event Deserialize(TEventDataSerializationType obj, string type);
    }
}