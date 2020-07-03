namespace Domainr.EventStore.Sql.Models
{
    internal sealed class EventEntity<TSerializationType>
    {
        public string Id { get; set; }

        public long Version { get; set; }

        public string Type { get; set; }

        public string AggregateRootId { get; set; }

        public TSerializationType Data { get; set; }
    }
}