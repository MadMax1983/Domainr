using System.Data;
using Domainr.EventStore.Serializers;
using Domainr.EventStore.Sql.Configuration;
using Domainr.EventStore.Sql.Data;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace Domainr.EventStore.Sql.Tests.Doubles
{
    public sealed class SqliteEventStore<TEventDataSerializationType>
        : SqlEventStore<TEventDataSerializationType>
    {
        public SqliteEventStore(
            ILogger<SqliteEventStore<TEventDataSerializationType>> logger,
            EventStoreSettings settings,
            ISqlStatementsLoader sqlStatementsLoader,
            IEventDataSerializer<TEventDataSerializationType> eventDataSerializer)
            : base(logger, settings, sqlStatementsLoader, eventDataSerializer)
        {
        }

        protected override IDbConnection CreateConnection(string connectionString)
        {
            return new SqliteConnection(connectionString);
        }
    }
}