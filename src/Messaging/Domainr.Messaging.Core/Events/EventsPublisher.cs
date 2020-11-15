using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domainr.Core.EventSourcing.Abstraction;
using Domainr.Messaging.Azure.ServiceBus;
using Domainr.Messaging.Core.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Domainr.Messaging.Core.Events
{
    public abstract class EventsPublisher
        : IEventsPublisher
    {
        protected EventsPublisher(ILogger<EventsPublisher> logger, IContainer container)
        {
            Logger = logger;

            Container = container;
        }

        protected ILogger<EventsPublisher> Logger { get; }

        protected IContainer Container { get; }

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

            using (var scope = Container.CreateScope())
            {
                switch (mode)
                {
                    case Mode.Default:
                        await PublishLocallyAsync(eventStream, scope, cancellationToken);
                        await PublishRemoteAsync(eventStream, scope, cancellationToken);

                        break;
                    case Mode.Local:
                        await PublishLocallyAsync(eventStream, scope, cancellationToken);

                        break;
                    case Mode.Remote:
                        await PublishRemoteAsync(eventStream, scope, cancellationToken);

                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
                }
            }
        }

        protected static Type GetEventHandlerType<TEvent>(TEvent @event)
            where TEvent : Event
        {
            return typeof(IEventListener<>).MakeGenericType(@event.GetType());
        }

        protected virtual async Task PublishLocallyAsync(IReadOnlyCollection<Event> eventStream, IScope scope, CancellationToken cancellationToken)
        {
            foreach (var @event in eventStream)
            {
                await PublishLocallyAsync(@event, scope, cancellationToken);
            }
        }

        protected virtual async Task PublishLocallyAsync<TEvent>(TEvent @event, IScope scope, CancellationToken cancellationToken)
            where TEvent : Event
        {
            var eventHandlerType = GetEventHandlerType(@event);

            var eventHandlers = scope.Container.GetServices(eventHandlerType).ToList();

            foreach (var eventHandler in eventHandlers)
            {
                try
                {
                    var handleMethod = eventHandlerType.GetMethod("OnAsync");

                    await (Task)handleMethod.Invoke(eventHandler, new object[] { @event, cancellationToken });
                }
                catch (Exception ex)
                {
                    // TODO: Add retry policy here
                    // TODO: Add logging here
                }
            }
        }


        protected abstract Task PublishRemoteAsync(IReadOnlyCollection<Event> eventStream, IScope scope, CancellationToken cancellationToken);
    }
}