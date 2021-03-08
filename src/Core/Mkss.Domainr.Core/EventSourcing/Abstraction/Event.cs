using System;
using Domainr.Core.Domain.Model;
using Domainr.Core.Exceptions;
using Domainr.Core.Infrastructure;
using Domainr.Core.Resources;

namespace Domainr.Core.EventSourcing.Abstraction
{
    [Serializable]
    public abstract class Event
    {
        private long _version;

        /// <summary>
        /// Use this constructor only for deserialization form an event store.
        /// </summary>
        protected Event()
        {
            _version = Constants.INITIAL_VERSION;
        }

        /// <summary>
        /// Use this constructor only for creation of a new event.
        /// </summary>
        /// <param name="aggregateRootId"></param>
        protected Event(string aggregateRootId)
            : this()
        {
            if (string.IsNullOrWhiteSpace(aggregateRootId))
            {
                throw new AggregateRootIdException("Aggregate root identifier cannot be null or empty string.");
            }

            AggregateRootId = aggregateRootId;
        }

        public string AggregateRootId { get; }

        public long Version => _version;

        internal void IncrementVersion(ref long aggregateRootVersion)
        {
            AggregateRootVersionValidator.Validate(aggregateRootVersion);

            _version = ++aggregateRootVersion;
        }
    }
}