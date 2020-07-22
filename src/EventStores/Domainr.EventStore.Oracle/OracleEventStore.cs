using System.Data.Common;
using Domainr.EventStore.Serializers;
using Domainr.EventStore.Sql;
using Domainr.EventStore.Sql.Configuration;
using Domainr.EventStore.Sql.Data;
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

        protected override DbConnection CreateConnection(string connectionString)
        {
            return new OracleConnection(connectionString);
        }
    }
}