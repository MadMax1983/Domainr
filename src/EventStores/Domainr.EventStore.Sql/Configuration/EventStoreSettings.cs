using System.Collections.Generic;

namespace Domainr.EventStore.Sql.Configuration
{
    public sealed class EventStoreSettings
    {
        public IReadOnlyDictionary<string, string> ConnectionStrings { get; set; }
    }
}