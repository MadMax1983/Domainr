using System.Data;
using Domainr.EventStore.Sql;
using Domainr.EventStore.Sql.Configuration;
using Domainr.EventStore.Sql.Data;
using Domainr.EventStore.Sql.Infrastructure;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;

namespace Domainr.EventStore.Oracle
{
    public sealed class OracleEventStore<TSerializationType>
        : SqlEventStore<TSerializationType>
    {
        public OracleEventStore(
            ILogger<OracleEventStore<TSerializationType>> logger,
            EventStoreSettings settings,
            ISqlStatementsLoader sqlStatementsLoader,
            IEventDataSerializer<TSerializationType> eventDataSerializer)
            : base(logger, settings, sqlStatementsLoader, eventDataSerializer)
        {
        }

        protected override IDbConnection CreateConnection(string connectionString)
        {
            return new OracleConnection(connectionString);
        }
    }
}