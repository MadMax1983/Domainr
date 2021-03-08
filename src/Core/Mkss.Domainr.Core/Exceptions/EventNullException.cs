using System;
using System.Runtime.Serialization;

namespace Domainr.Core.Exceptions
{
    [Serializable]
    public class EventNullException
        : Exception
    {
        public EventNullException()
        {
        }

        public EventNullException(string message)
            : base(message)
        {
        }

        public EventNullException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected EventNullException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}