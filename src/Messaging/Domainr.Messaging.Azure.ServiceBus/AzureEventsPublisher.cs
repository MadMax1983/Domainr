using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Domainr.Core.EventSourcing.Abstraction;
using Domainr.Messaging.Core.DependencyInjection;
using Domainr.Messaging.Core.Events;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Domainr.Messaging.Azure.ServiceBus
{
    public sealed class AzureEventsPublisher
        : EventsPublisher
    {
        private readonly ITopicClient _topicClient;

        public AzureEventsPublisher(ILogger<EventsPublisher> logger, IContainer container, ITopicClient topicClient)
            : base(logger, container)
        {
            _topicClient = topicClient;
        }

        protected override async Task PublishRemoteAsync(IReadOnlyCollection<Event> eventStream, IScope scope, CancellationToken cancellationToken)
        {
            var messages = eventStream
                .Select(@event =>
                {
                    var eventJson = JsonConvert.SerializeObject(@event);

                    var eventBytes = Encoding.UTF8.GetBytes(eventJson);

                    var eventMessage = new Message(eventBytes);

                    eventMessage.UserProperties.Add("event-type", @event.GetType().Name);

                    return eventMessage;
                })
                .ToList();

            await _topicClient.SendAsync(messages);
        }
    }
}