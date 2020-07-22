using System.Data.Common;
using System.Data.SqlClient;
using Domainr.EventStore.Serializers;
using Domainr.EventStore.Sql;
using Domainr.EventStore.Sql.Configuration;
using Domainr.EventStore.Sql.Data;
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

        protected override DbConnection CreateConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }
    }
}