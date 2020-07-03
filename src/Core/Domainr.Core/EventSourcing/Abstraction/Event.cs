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
        /// <summary>
        /// Use this constructor only for deserialization form an event store.
        /// </summary>

        protected Event()
        {
            Version = Constants.INITIAL_VERSION;
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
                throw new AggregateRootIdException(ExceptionResources.EmptyAggregateRootId);
            }

            AggregateRootId = aggregateRootId;
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