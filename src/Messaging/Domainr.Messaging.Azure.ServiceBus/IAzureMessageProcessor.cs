using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Domainr.Core.EventSourcing.Abstraction;

namespace Domainr.Messaging.Azure.ServiceBus
{
    public interface IAzureMessageProcessor
    {
        Task SendAsync<TCommand>(TCommand command, Mode mode = Mode.Default, CancellationToken cancellationToken = default)
            where TCommand : ICommand;

        Task PublishAsync(IReadOnlyCollection<Event> eventStream, Mode mode = Mode.Default, CancellationToken cancellationToken = default);
    }
}