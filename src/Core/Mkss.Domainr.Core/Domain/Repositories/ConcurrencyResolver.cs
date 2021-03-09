using System;
using System.Collections.Generic;
using System.Linq;
using Domainr.Core.EventSourcing.Abstraction;
using Domainr.Core.Exceptions;

namespace Domainr.Core.Domain.Repositories
{
    [Obsolete("Concurrency check should be moved to an event store implementation, with default behavior of throwing AggregateRootConcurrencyException")]
    public sealed class ConcurrencyResolver
        : IConcurrencyResolver
    {
        private readonly Dictionary<Type, IReadOnlyCollection<Type>> _conflictRegister;

        public ConcurrencyResolver()
        {
            _conflictRegister = new Dictionary<Type, IReadOnlyCollection<Type>>();
        }

        public bool ConflictsWith(Type eventType, IReadOnlyCollection<Type> concurrentEventTypes)
        {
            // If (event)type is not registered
            // it means that it conflicts with all other (event)types.
            //
            // OR
            //
            // If in registered-conflict-(event)types we have any of the passed-concurrent-(event)types
            // then it conflicts with current (event)type as well.

            return !_conflictRegister.ContainsKey(eventType) ||
                   concurrentEventTypes
                       .Any(concurrentEvent => _conflictRegister[eventType]
                           .Any(et => et == concurrentEvent));
        }

        public void RegisterConflictList(Type eventType, IReadOnlyCollection<Type> conflictsWith)
        {
            var defaultEventType = typeof(Event);

            if (!eventType.IsSubclassOf(defaultEventType))
            {
                throw new InvalidEventTypeException($"Event definition must be of type {defaultEventType.Name}.");
            }

            if (conflictsWith.Any(c => !c.IsSubclassOf(defaultEventType)))
            {
                throw new InvalidEventTypeException($"All conflicts with type must be of type {defaultEventType.Name}.");
            }

            if (_conflictRegister.ContainsKey(eventType))
            {
                _conflictRegister.Remove(eventType);
            }

            _conflictRegister.Add(eventType, conflictsWith);
        }
    }
}