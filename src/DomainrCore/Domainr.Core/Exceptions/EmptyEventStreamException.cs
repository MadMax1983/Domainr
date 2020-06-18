using System;
using System.Runtime.Serialization;

namespace Domainr.Core.Exceptions
{
    [Serializable]
    public class EmptyEventStreamException
        : Exception
    {
        public EmptyEventStreamException()
        {
        }

        public EmptyEventStreamException(string message)
            : base(message)
        {
        }

        public EmptyEventStreamException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected EmptyEventStreamException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}