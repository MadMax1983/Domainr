using System.Threading;
using System.Threading.Tasks;

namespace Domainr.Messaging.Core.Commands
{
    public interface ICommandHandler<in TCommand>
        where TCommand : ICommand
    {
        Task HandleAsync(TCommand command, CancellationToken cancellationToken = default);
    }
}