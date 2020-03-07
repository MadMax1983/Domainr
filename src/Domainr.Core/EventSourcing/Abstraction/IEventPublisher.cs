using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domainr.Core.EventSourcing.Abstraction
{
    public interface IEventPublisher
    {
        Task PublishAsync(IReadOnlyCollection<Event> eventStream);
    }
}