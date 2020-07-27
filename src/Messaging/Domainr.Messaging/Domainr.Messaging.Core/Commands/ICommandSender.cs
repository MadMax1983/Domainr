using System.Threading;
using System.Threading.Tasks;

namespace Domainr.Messaging.Core.Commands
{
    public interface ICommandSender
    {
        Task SendLocallyAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
            where TCommand : ICommand;

        Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
            where TCommand : ICommand;

    }
}