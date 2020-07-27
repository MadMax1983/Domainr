using System.Threading.Tasks;
using Domainr.Core.EventSourcing.Abstraction;

namespace Domainr.Messaging.Core.Events
{
    public interface IEventBus
    {
        Task PublishAsync<TEvent>(TEvent @event)
            where TEvent : Event, IEvent;

        Task PublishLocallyAsync<TEvent>(TEvent @event)
            where TEvent : Event, IEvent;
    }
}