﻿using System.Threading.Tasks;
using Domainr.Core.Domain.Model;

namespace Domainr.Core.Domain.Repositories
{
    public interface IRepository<TAggregateRoot, in TAggregateRootId>
        where TAggregateRoot : AggregateRoot<TAggregateRootId>
        where TAggregateRootId : IAggregateRootId
    {
        Task<TAggregateRoot> GetByIdAsync(TAggregateRootId id);

        Task SaveAsync(TAggregateRoot aggregateRoot, long expectedVersion);
    }
}