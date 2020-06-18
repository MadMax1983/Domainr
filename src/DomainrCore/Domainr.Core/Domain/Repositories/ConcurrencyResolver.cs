using System;
using System.Collections.Generic;
using System.Linq;
using Domainr.Core.EventSourcing.Abstraction;
using Domainr.Core.Exceptions;
using Domainr.Core.Resources;

namespace Domainr.Core.Domain.Repositories
{
    public sealed class ConcurrencyResolver
        : IConcurrencyResolver
    {
        private readonly Dictionary<Type, IReadOnlyCollection<Type>> _conflictRegister;

        public ConcurrencyResolver()
        {
            _conflictRegister = new Dictionary<Type, IReadOnlyCollection<Type>>();
        }

        public bool ConflictsWith(Type eventType, IReadOnlyCollection<Type> previousEventTypes)
        {
            return !_conflictRegister.ContainsKey(eventType) ||
                   previousEventTypes.Any(previousEvent => _conflictRegister[eventType].Any(et => et == previousEvent));
        }

        public void RegisterConflictList(Type eventType, IReadOnlyCollection<Type> conflictsWith)
        {
            var defaultEventType = typeof(Event);

            if (!eventType.IsSubclassOf(defaultEventType))
            {
                throw new InvalidEventTypeException(string.Format(ExceptionResources.InvalidEventType, defaultEventType.Name));
            }

            if (conflictsWith.Any(c => !c.IsSubclassOf(defaultEventType)))
            {
                throw new InvalidEventTypeException(string.Format(ExceptionResources.InvalidConflictEventTypes, defaultEventType.Name));
            }

            if (_conflictRegister.ContainsKey(eventType))
            {
                _conflictRegister.Remove(eventType);
            }

            _conflictRegister.Add(eventType, conflictsWith);
        }
    }
}