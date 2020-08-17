using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domainr.Core.EventSourcing.Abstraction;
using Domainr.Messaging.Core.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Domainr.Messaging.Core.Events
{
    public class LocalEventPublisher
        : IEventPublisher
    {
        public LocalEventPublisher(ILogger<LocalEventPublisher> logger, IContainer container)
        {
            Logger = logger;

            Container = container;
        }

        protected ILogger<LocalEventPublisher> Logger { get; }

        protected IContainer Container { get; }

        public async Task PublishAsync(IReadOnlyCollection<Event> eventStream, CancellationToken cancellationToken = default)
        {
            if (eventStream == null)
            {
                throw new ArgumentNullException(nameof(eventStream));
            }

            if (!eventStream.Any())
            {
                throw new ArgumentException(); // TODO: Add custom message.
            }

            foreach (var @event in eventStream)
            {
                try
                {
                    await Publish(@event, cancellationToken);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, ex.Message); // TODO: Add custom message here.
                }
            }
        }

        protected static Type GetEventHandlerType<TEvent>(TEvent @event)
            where TEvent : Event
        {
            return typeof(IEventListener<>).MakeGenericType(@event.GetType());
        }

        protected virtual async Task Publish<TEvent>(TEvent @event, CancellationToken cancellationToken)
            where TEvent : Event
        {
            var eventHandlerType = GetEventHandlerType(@event);

            var eventHandlers = Container.GetServices(eventHandlerType).ToList();

            foreach (var eventHandler in eventHandlers)
            {
                var handleMethod = eventHandlerType.GetMethod("OnAsync");

                await (Task)handleMethod.Invoke(eventHandler, new object[] { @event, cancellationToken });
            }
        }
    }
}