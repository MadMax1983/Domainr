using System.Threading;
using System.Threading.Tasks;

namespace Domainr.Messaging.Core.Commands
{
    public interface ILocalCommandSender
    {
        Task SendLocallyAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
            where TCommand : ICommand;
    }
}