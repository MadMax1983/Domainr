namespace Domainr.EventStore.Sql.Tests.Doubles
{
    internal sealed class EventEntity
    {
        public string Id { get; }

        public long Version { get; }

        public string AggregateRootId { get; }

        public string Type { get; }

        public string Data { get; }

        public EventEntity(string id, long version, string aggregateRootId, string type, string data)
        {
            Id = id;

            Version = version;
            AggregateRootId = aggregateRootId;

            Type = type;

            Data = data;
        }
    }
}