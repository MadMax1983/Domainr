using System;
using System.Runtime.Serialization;

namespace Domainr.Core.Exceptions
{
    [Serializable]
    public class InvalidEventTypeException
        : Exception
    {
        public InvalidEventTypeException()
        {
        }

        public InvalidEventTypeException(string message)
            : base(message)
        {
        }

        public InvalidEventTypeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected InvalidEventTypeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}