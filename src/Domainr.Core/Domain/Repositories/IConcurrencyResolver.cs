using System;
using System.Collections.Generic;

namespace Domainr.Core.Domain.Repositories
{
    public interface IConcurrencyResolver
    {
        bool ConflictsWith(Type eventType, IReadOnlyCollection<Type> previousEventTypes);

        void RegisterConflictList(Type eventType, IReadOnlyCollection<Type> conflictsWith);
    }
}