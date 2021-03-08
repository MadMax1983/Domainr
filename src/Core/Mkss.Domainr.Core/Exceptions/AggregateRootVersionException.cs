using System;
using System.Runtime.Serialization;

namespace Domainr.Core.Exceptions
{
    [Serializable]
    public class AggregateRootVersionException
        : Exception
    {
        public AggregateRootVersionException()
        {
        }

        public AggregateRootVersionException(string message)
            : base(message)
        {
        }

        public AggregateRootVersionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected AggregateRootVersionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}