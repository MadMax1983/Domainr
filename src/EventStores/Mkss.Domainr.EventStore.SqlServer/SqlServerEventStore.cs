using System;
using System.Data.SqlClient;
using Domainr.EventStore.Sql;
using Domainr.EventStore.Sql.Configuration;
using Domainr.EventStore.Sql.Data;
using Domainr.EventStore.Sql.Factories;
using Domainr.EventStore.Sql.Serializers;

namespace Domainr.EventStore
{
    public sealed class SqlServerEventStore<TSerializationType>
        : SqlEventStore<TSerializationType>
    {
        public SqlServerEventStore(EventStoreSettings settings, ISqlStatementsLoader sqlStatementsLoader, IDbConnectionFactory connectionFactory, IEventDataSerializer<TSerializationType> eventDataSerializer)
            : base(settings, sqlStatementsLoader, connectionFactory, eventDataSerializer)
        {
        }

        protected override bool IsConcurrencyException(Exception ex)
        {
            const int PRIMARY_KEY_VIOLATION_CODE = 2627;
            const int UNIQE_INDEX_VIOLATION_CODE = 2601;

            return ex is SqlException sqlException &&
                   (sqlException.Number == PRIMARY_KEY_VIOLATION_CODE ||
                    sqlException.Number == UNIQE_INDEX_VIOLATION_CODE);
        }
    }
}