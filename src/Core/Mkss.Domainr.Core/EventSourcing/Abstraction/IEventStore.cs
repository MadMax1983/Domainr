﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Domainr.Core.EventSourcing.Abstraction
{
    public interface IEventStore
    {
        Task<IReadOnlyCollection<Event>> GetByAggregateRootIdAsync(string aggregateRootId, long fromVersion, CancellationToken cancellationToken = default);

        Task SaveAsync(IReadOnlyCollection<Event> events, string eventMetadata = default, CancellationToken cancellationToken = default);
    }
}