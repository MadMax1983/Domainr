namespace Domainr.EventStore.Models
{
    public sealed class EventEntity
    {
        public string Id { get; set; }

        public long Version { get; set; }

        public string AggregateRootId { get; set; }

        public string Data { get; set; }
    }
}