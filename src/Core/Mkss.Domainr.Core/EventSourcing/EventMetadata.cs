using System;

namespace Domainr.Core.EventSourcing
{
    public class EventMetadata
    {
        public EventMetadata(string correlationId, string streamId, string user, DateTime timestampUtc)
        {
            CorrelationId = correlationId;
            StreamId = streamId;
            
            User = user;
            
            TimestampUtc = timestampUtc;
        }
        
        public string CorrelationId { get; }
        
        public string StreamId { get; }

        public string User { get; }
        
        public DateTime TimestampUtc { get; }
    }
}