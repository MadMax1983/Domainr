using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Domainr.Core.EventSourcing.Abstraction;
using Domainr.Messaging.Core;
using Domainr.Messaging.Core.Commands;
using Domainr.Messaging.Core.DependencyInjection;
using Domainr.Messaging.Core.Events;
using Microsoft.Azure.ServiceBus;
using ICommand = System.Windows.Input.ICommand;

namespace Domainr.Messaging.Azure.ServiceBus
{
    public class AzureMessageProcessor
    {
        private readonly IContainer _container;

        private readonly ITopicClient _topicClient;

        public AzureMessageProcessor(IContainer container, ITopicClient topicClient)
        {
            _container = container;

            _topicClient = topicClient;
        }

        public async Task SendAsync<TCommand>(TCommand command, Mode mode = Mode.Default, CancellationToken cancellationToken = default)
            where TCommand : ICommand
        {
            if (mode == Mode.Local)
            {
                await SendAsync(
                    command,
                    () => throw new InvalidOperationException($"Can't find command handler for command: {typeof(TCommand).Name}"),
                    cancellationToken);
            }
            else if (mode == Mode.Remote)
            {
                await SendRemoteMessageAsync(command, SetCommandProperties<TCommand>);
            }
            else
            {
                await SendAsync(
                    command,
                    () => SendRemoteMessageAsync(command, SetCommandProperties<TCommand>),
                    cancellationToken);
            }
        }

        public async Task PublishAsync(IReadOnlyCollection<Event> eventStream, Mode mode = Mode.Default, CancellationToken cancellationToken = default)
        {
            if (eventStream == null)
            {
                throw new ArgumentNullException(nameof(eventStream));
            }

            if (!eventStream.Any())
            {
                throw new ArgumentException(); // TODO: Add custom message.
            }

            if (mode == Mode.Local)
            {
                foreach (var @event in eventStream)
                {
                    await PublishEventLocallyAsync(@event, cancellationToken);
                }
            }
            else if (mode == Mode.Remote)
            {
                foreach (var @event in eventStream)
                {
                    await SendRemoteMessageAsync(@event, message => SetEventProperties(message, @event.GetType()));
                }
            }
            else
            {
                // TODO: Make it parallel here.

                foreach (var @event in eventStream)
                {
                    await PublishEventLocallyAsync(@event, cancellationToken);

                    await SendRemoteMessageAsync(@event, message => SetEventProperties(message, @event.GetType()));
                }
            }

            await Task.CompletedTask;
        }

        private static void SetCommandProperties<TCommand>(Message message)
    where TCommand : ICommand
        {
            message.UserProperties.Add("command-type", typeof(TCommand).Name);
            message.UserProperties.Add("recipient", $"{typeof(TCommand).Name}Handler");
        }

        private static void SetEventProperties(Message message, Type eventType)
        {
            message.UserProperties.Add("event-type", eventType.Name);
        }

        private async Task SendAsync<TCommand>(TCommand command, Func<Task> invokeWhenNullAsync, CancellationToken cancellationToken = default)
            where TCommand : ICommand
        {
            var commandHandler = _container.GetService<ICommandHandler<TCommand>>();
            if (commandHandler == null)
            {
                await invokeWhenNullAsync();
            }
            else
            {
                // TODO: Create scope here.
                await commandHandler.HandleAsync(command, cancellationToken);
            }
        }

        private async Task PublishEventLocallyAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
            where TEvent : Event
        {
            var eventListenerType = typeof(IEventListener<>).MakeGenericType(@event.GetType());

            var eventListeners = _container.GetServices(eventListenerType);

            // TODO: Create scope here.
            foreach (var eventListener in eventListeners)
            {
                var onMethod = eventListenerType.GetMethod("OnAsync");

                await (Task)onMethod.Invoke(eventListener, new object[] { @event, cancellationToken });
            }
        }

        private async Task SendRemoteMessageAsync<TMessage>(TMessage message, Action<Message> setUserProperties)
            where TMessage : IMessage
        {
            var messageJson = Newtonsoft.Json.JsonConvert.SerializeObject(message);

            var messageBytes = Encoding.UTF8.GetBytes(messageJson);

            var messageObj = new Message(messageBytes);

            setUserProperties(messageObj);

            await _topicClient.SendAsync(messageObj);
        }
    }
}