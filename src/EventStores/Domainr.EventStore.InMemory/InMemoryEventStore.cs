using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domainr.Core.EventSourcing.Abstraction;

namespace Domainr.EventStore.InMemory
{
    public sealed class InMemoryEventStore
        : IEventStore
    {
        private static readonly List<Event> Store = new List<Event>();

        public Task<IReadOnlyCollection<Event>> GetByAggregateRootIdAsync(string aggregateRootId, long fromVersion)
        {
            var result = Store
                .Where(e => e.AggregateRootId.Equals(aggregateRootId))
                .OrderBy(e => e.Version)
                .Select(e => e)
                .ToList();

            return Task.FromResult<IReadOnlyCollection<Event>>(result);
        }

        public Task SaveAsync(IReadOnlyCollection<Event> events)
        {
            foreach (var @event in events)
            {
                Store.Add(@event);
            }

            return Task.CompletedTask;
        }
    }
}