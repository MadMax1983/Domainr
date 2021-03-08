using System;
using System.Collections.Generic;

namespace Domainr.Core.Domain.Repositories
{
    [Obsolete("Concurrency check should be moved to an event store implementation, with default behavior of throwing AggregateRootConcurrencyException")]
    public interface IConcurrencyResolver
    {
        bool ConflictsWith(Type eventType, IReadOnlyCollection<Type> concurrentEventTypes);

        void RegisterConflictList(Type eventType, IReadOnlyCollection<Type> conflictsWith);
    }
}