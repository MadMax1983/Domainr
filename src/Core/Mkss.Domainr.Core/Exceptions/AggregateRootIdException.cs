using System;
using System.Runtime.Serialization;

namespace Domainr.Core.Exceptions
{
    [Serializable]
    public class AggregateRootIdException
        : Exception
    {
        public AggregateRootIdException()
        {
        }

        public AggregateRootIdException(string message)
            : base(message)
        {
        }

        public AggregateRootIdException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected AggregateRootIdException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}