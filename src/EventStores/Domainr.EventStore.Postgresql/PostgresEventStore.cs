using System.Data.Common;
using Domainr.EventStore.Serializers;
using Domainr.EventStore.Sql;
using Domainr.EventStore.Sql.Configuration;
using Domainr.EventStore.Sql.Data;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Domainr.EventStore.Postgresql
{
    public sealed class PostgresEventStore<TSerializationType>
        : SqlEventStore<TSerializationType>
    {
        public PostgresEventStore(
            ILogger<PostgresEventStore<TSerializationType>> logger,
            EventStoreSettings settings,
            ISqlStatementsLoader sqlStatementsLoader,
            IEventDataSerializer<TSerializationType> eventDataSerializer)
            : base(logger, settings, sqlStatementsLoader, eventDataSerializer)
        {
        }

        protected override DbConnection CreateConnection(string connectionString)
        {
            return new NpgsqlConnection(connectionString);
        }
    }
}