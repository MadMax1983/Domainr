using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domainr.Core.EventSourcing.Abstraction;
using Domainr.Messaging.Azure.ServiceBus;

namespace Domainr.Messaging.Core.Events
{
    public interface IEventsPublisher
    {
        Task PublishAsync(IReadOnlyCollection<Event> eventStream, Mode mode = Mode.Default, CancellationToken cancellationToken = default);
    }
}