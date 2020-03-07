using System;
using Domainr.Core.Domain.Model;
using Domainr.Core.Infrastructure;

namespace Domainr.Core.EventSourcing.Abstraction
{
    [Serializable]
    public abstract class Event
    {
        /// <summary>
        /// Use this constructor only for deserialization form an event store.
        /// </summary>

        protected Event()
        {
        }

        /// <summary>
        /// Use this constructor only for creation of a new event.
        /// </summary>
        /// <param name="aggregateRootId"></param>

        protected Event(string aggregateRootId)
        {
            AggregateRootId = aggregateRootId;

            Version = Constants.INITIAL_VERSION;
        }

        public string AggregateRootId { get; }

        public long Version { get; private set; }

        internal void IncrementVersion(ref long aggregateRootVersion)
        {
            AggregateRootVersionValidator.Validate(aggregateRootVersion);

            Version = ++aggregateRootVersion;
        }
    }
}