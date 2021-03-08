using System;
using Domainr.EventStore.Sql.Configuration;
using Domainr.EventStore.Sql.Data;
using Domainr.EventStore.Sql.Factories;
using Domainr.EventStore.Sql.Serializers;

namespace Domainr.EventStore.Sql.Tests.Doubles
{
    internal sealed class TestSqlEventStore<TSerializationType>
        : SqlEventStore<TSerializationType>
    {
        public TestSqlEventStore(
            EventStoreSettings settings,
            ISqlStatementsLoader sqlStatementsLoader,
            IDbConnectionFactory connectionFactory,
            IEventDataSerializer<TSerializationType> eventDataSerializer)
            : base(
                settings,
                sqlStatementsLoader,
                connectionFactory,
                eventDataSerializer)
        {
        }

        protected override bool IsConcurrencyException(Exception ex)
        {
            return false;
        }
    }
}