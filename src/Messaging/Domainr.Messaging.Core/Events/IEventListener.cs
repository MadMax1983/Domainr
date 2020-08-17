using System.Threading;
using System.Threading.Tasks;
using Domainr.Core.EventSourcing.Abstraction;

namespace Domainr.Messaging.Core.Events
{
    public interface IEventListener<in TEvent>
        where TEvent : Event, IEvent
    {
        Task OnAsync(TEvent @event, CancellationToken cancellationToken = default);
    }
}