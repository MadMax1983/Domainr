using System.Data;
using System.Data.SqlClient;
using Domainr.EventStore.Sql;
using Domainr.EventStore.Sql.Configuration;
using Domainr.EventStore.Sql.Data;
using Domainr.EventStore.Sql.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Domainr.EventStore
{
    public sealed class SqlServerEventStore<TEventDataSerializationType>
        : SqlEventStore<TEventDataSerializationType>
    {
        public SqlServerEventStore(
            ILogger<SqlServerEventStore<TEventDataSerializationType>> logger,
            EventStoreSettings settings,
            ISqlStatementsLoader sqlStatementsLoader,
            IEventDataSerializer<TEventDataSerializationType> eventDataSerializer)
            : base(logger, settings, sqlStatementsLoader, eventDataSerializer)
        {
        }

        protected override IDbConnection CreateConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }
    }
}