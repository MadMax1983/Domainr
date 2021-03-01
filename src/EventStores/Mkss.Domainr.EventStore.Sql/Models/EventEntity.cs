using System.Diagnostics.CodeAnalysis;

namespace Domainr.EventStore.Sql.Models
{
    [ExcludeFromCodeCoverage]
    internal sealed class EventEntity<TSerializationType>
    {
        public string Type { get; set; }

        public TSerializationType Data { get; set; }
    }
}