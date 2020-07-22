using System.Data.Common;
using Domainr.EventStore.Serializers;
using Domainr.EventStore.Sql;
using Domainr.EventStore.Sql.Configuration;
using Domainr.EventStore.Sql.Data;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace Domainr.EventStore.MySql
{
    public sealed class MySqlEventStore<TSerializationType>
        : SqlEventStore<TSerializationType>
    {
        public MySqlEventStore(
            ILogger<MySqlEventStore<TSerializationType>> logger,
            EventStoreSettings settings,
            ISqlStatementsLoader sqlStatementsLoader,
            IEventDataSerializer<TSerializationType> eventDataSerializer)
            : base(logger, settings, sqlStatementsLoader, eventDataSerializer)
        {
        }

        protected override DbConnection CreateConnection(string connectionString)
        {
            return new MySqlConnection(connectionString);
        }
    }
}