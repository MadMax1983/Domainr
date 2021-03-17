using System.Collections.Generic;
using Domainr.Core.EventSourcing.Abstraction;

namespace Domainr.Core.Domain.Model
{
    public interface IAggregateRoot<out TId>
        where TId : class, IAggregateRootId
    {
        TId Id { get; }
        
        long Version { get; }
        
        void LoadFromStream(IReadOnlyCollection<Event> eventStream);

        IReadOnlyCollection<Event> GetUncommittedChanges();

        void CommitChanges();
    }
}