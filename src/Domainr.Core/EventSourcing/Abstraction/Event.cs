using System;
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
            if (aggregateRootVersion < Constants.INITIAL_VERSION)
            {
                throw new AggregateRootVersionException(string.Format(ExceptionResources.AggregateRootVersionIsInvalid, aggregateRootVersion, Constants.INITIAL_VERSION));
            }

            Version = ++aggregateRootVersion;
        }
    }
}