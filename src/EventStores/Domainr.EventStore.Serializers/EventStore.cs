using System.Collections.Generic;
using System.Threading.Tasks;
using Domainr.Core.EventSourcing.Abstraction;

namespace Domainr.EventStore.Serializers
{
    public abstract class EventStore
        : IEventStore
    {
        public virtual void SetVersion<TEvent>(TEvent @event, ref long aggregateRootVersion)
            where TEvent : Event
        {
            typeof(TEvent).GetProperty("Version")?.SetValue(@event, aggregateRootVersion);
        }

        public abstract Task<IReadOnlyCollection<Event>> GetByAggregateRootIdAsync(string aggregateRootId, long fromVersion);

        public abstract Task SaveAsync(IReadOnlyCollection<Event> events);
    }
}