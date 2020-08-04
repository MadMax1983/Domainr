using System;
using System.Threading;
using System.Threading.Tasks;
using Domainr.Messaging.Core.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Domainr.Messaging.Core.Commands
{
    public class LocalCommandSender
        : ILocalCommandSender
    {
        public LocalCommandSender(ILogger<LocalCommandSender> logger, IContainer container)
        {
            Logger = logger;

            Container = container;
        }

        protected ILogger<LocalCommandSender> Logger { get; }

        protected IContainer Container { get; }

        public virtual async Task SendLocallyAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
            where TCommand : ICommand
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            var commandHandler = Container.GetService<ICommandHandler<TCommand>>();
            if (commandHandler == null)
            {
                throw new Exception(); // TODO: Add message and custom exception here.
            }

            try
            {
                await commandHandler.HandleAsync(command, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);

                throw;
            }
        }
    }
}