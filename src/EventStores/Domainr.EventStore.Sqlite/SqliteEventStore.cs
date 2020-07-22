using System.Data.Common;
using Domainr.EventStore.Serializers;
using Domainr.EventStore.Sql;
using Domainr.EventStore.Sql.Configuration;
using Domainr.EventStore.Sql.Data;
using Microsoft.Extensions.Logging;

namespace Domainr.EventStore.Sqlite
{
    public sealed class SqliteEventStore<TEventDataSerializationType>
        : SqlEventStore<TEventDataSerializationType>
    {
        private readonly DbConnection _mockConnection;

        public SqliteEventStore(
            ILogger<SqliteEventStore<TEventDataSerializationType>> logger,
            EventStoreSettings settings,
            ISqlStatementsLoader sqlStatementsLoader,
            IEventDataSerializer<TEventDataSerializationType> eventDataSerializer,
            DbConnection mockConnection)
            : base(logger, settings, sqlStatementsLoader, eventDataSerializer)
        {
            _mockConnection = mockConnection;
        }

        protected override DbConnection CreateConnection(string connectionString)
        {
            return _mockConnection;
        }
    }
}