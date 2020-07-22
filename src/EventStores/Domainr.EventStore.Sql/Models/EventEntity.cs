namespace Domainr.EventStore.Sql.Models
{
    internal sealed class EventEntity<TSerializationType>
    {
        public string Type { get; set; }

        public TSerializationType Data { get; set; }
    }
}