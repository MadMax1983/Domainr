using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Domainr.EventStore.Sql.Configuration
{
    [ExcludeFromCodeCoverage]
    public sealed class EventStoreSettings
    {
        public IReadOnlyDictionary<string, string> ConnectionStrings { get; set; }
    }
}