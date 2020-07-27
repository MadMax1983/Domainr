using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domainr.Core.EventSourcing.Abstraction;
using Domainr.Messaging.Core.Commands;
using Domainr.Messaging.Core.DependencyInjection;
using Domainr.Messaging.Core.Events;
using Domainr.Messaging.Core.Queries;
using Microsoft.Extensions.Logging;

namespace Domainr.Messaging.Core
{
    public abstract class LocalMessageBus :
        ICommandSender,
        IQueryBus,
        IEventBus
    {
        protected LocalMessageBus(ILogger<LocalMessageBus> logger, IContainer container)
        {
            Logger = logger;

            Container = container;
        }

        public ILogger<LocalMessageBus> Logger { get; }

        public IContainer Container { get; }

        public async Task SendLocallyAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
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

        public abstract Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
            where TCommand : ICommand;

        public async Task<TResult> QueryAsync<TResult, TQuery>(TQuery query, CancellationToken cancellationToken = default)
            where TQuery : IQuery<TResult>
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var queryHandler = Container.GetService<IQueryHandler<TQuery, TResult>>();
            if (queryHandler == null)
            {
                throw new Exception(); // TODO: Add message and custom exception here.
            }

            try
            {
                return await queryHandler.ExecuteAsync(query, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);

                throw;
            }
        }

        public async Task PublishLocallyAsync<TEvent>(TEvent @event)
            where TEvent : Event, IEvent
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            var eventListeners = Container.GetServices<IEventListener<TEvent>>();
            if (eventListeners == null)
            {
                throw new Exception(); // TODO: Add message and custom exception here.
            }

            if (eventListeners.Any())
            {
                foreach (var eventListener in eventListeners)
                {
                    try
                    {
                        await eventListener.OnAsync(@event);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, ex.Message); // TODO: Add custom message here.
                    }
                }
            }
        }

        public abstract Task PublishAsync<TEvent>(TEvent @event)
            where TEvent : Event, IEvent;
    }
}