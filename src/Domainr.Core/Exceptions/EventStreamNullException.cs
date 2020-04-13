using System;
using System.Runtime.Serialization;

namespace Domainr.Core.Exceptions
{
    [Serializable]
    public class EventStreamNullException
        : Exception
    {
        public EventStreamNullException()
        {
        }

        public EventStreamNullException(string message)
            : base(message)
        {
        }

        public EventStreamNullException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected EventStreamNullException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}