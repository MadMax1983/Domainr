using System.Threading;
using System.Threading.Tasks;

namespace Domainr.Messaging.Core.Commands
{
    public interface ICommandSender
    {
        Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
            where TCommand : ICommand;

    }
}