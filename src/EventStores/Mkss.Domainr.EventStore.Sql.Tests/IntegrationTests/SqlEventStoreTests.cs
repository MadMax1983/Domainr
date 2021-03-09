using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Domainr.Core.EventSourcing.Abstraction;
using Domainr.Core.Exceptions;
using Domainr.Core.Infrastructure;
using Domainr.EventStore.Sql.Configuration;
using Domainr.EventStore.Sql.Data;
using Domainr.EventStore.Sql.Factories;
using Domainr.EventStore.Sql.Serializers;
using Domainr.EventStore.Sql.Tests.Doubles;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Domainr.EventStore.Sql.Tests.IntegrationTests
{
    public abstract class SqlEventStoreTests<TDbConnectionFactory>
        where TDbConnectionFactory : IDbConnectionFactory
    {
        private const string CREATE_DATABASE_SQL_STATEMENT = "CREATE DATABASE [EventStoreTests]";

        private const string DROP_DATABASE_SQL_STATEMENT = "ALTER DATABASE [EventStoreTests] SET  SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [EventStoreTests]";

        protected EventStoreSettings Settings { get; } = new EventStoreSettings();

        protected ISqlStatementsLoader SqlStatementsLoader { get; } = new SqlStatementsLoader();

        protected Mock<IEventDataSerializer<string>> MockEventDataSerializer { get; }= new Mock<IEventDataSerializer<string>>();

        protected virtual async Task OneTimeSetUpInternal(string initialConnectionString, string connectionString)
        {
            Settings.ConnectionStrings = new Dictionary<string, string>
            {
                {"EventStoreInitialize", initialConnectionString},
                {"EventStore", connectionString},
            };
            
            SqlStatementsLoader.LoadScripts();

            await CreateDatabaseAsync<SqlConnection>(Settings.ConnectionStrings["EventStoreInitialize"]);

            MockEventDataSerializer
                .Setup(m => m.Serialize(It.IsAny<Event>()))
                .Returns((Event @event) => JsonConvert.SerializeObject(@event));

            MockEventDataSerializer
                .Setup(m => m.Deserialize(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string jsonString, string type) =>
                {
                    var jObj = JObject.Parse(jsonString);

                    var version = jObj.GetValue("Version").Value<long>();

                    var @event = (TestEvent)JsonConvert.DeserializeObject(jsonString, Type.GetType(type));

                    @event.SetVersion(version);

                    return @event;
                });
        }

        protected virtual async Task OneTimeTearDownInternal()
        {
            await DropDatabaseAsync<SqlConnection>(Settings.ConnectionStrings["EventStoreInitialize"]);
        }

        protected async Task ExecuteConcurrencyCheckTest(IEventStoreInitializer eventStoreInitializer, IEventStore eventStore)
        {
            // Arrange
            var events = new List<TestEvent>
            {
                new TestEvent("arid", "sp1"),
                new TestEvent("arid", "sp2")
            };

            events[0].SetVersion(0);
            events[1].SetVersion(0);

            // Act
            await eventStoreInitializer.InitializeAsync();

            Func<Task> act = async () => await eventStore.SaveAsync(events);

            // Assert
            act.Should()
                .Throw<ConcurrencyException>();
        }

        protected async Task ExecuteTest(IEventStoreInitializer eventStoreInitializer, IEventStore eventStore)
        {
            // Arrange
            var events = new List<TestEvent>
            {
                new TestEvent("arid", "sp1"),
                new TestEvent("arid", "sp2"),
                new TestEvent("arid", "sp3")
            };

            for (var i = 0; i < events.Count; i++)
            {
                events[i].SetVersion(i);
            }

            // Act
            await eventStoreInitializer.InitializeAsync();

            await eventStore.SaveAsync(events);

            var queriedEvents = await eventStore.GetByAggregateRootIdAsync("arid", Constants.INITIAL_VERSION);

            // Assert
            queriedEvents
                .Should()
                .BeEquivalentTo(events);

        }

        protected virtual async Task CreateDatabaseAsync<TDbConnection>(string connectionString)
            where TDbConnection : IDbConnection, new()
        {
            await ExecuteSqlStatementAsync<TDbConnection>(connectionString, CREATE_DATABASE_SQL_STATEMENT);
        }

        protected virtual async Task DropDatabaseAsync<TDbConnection>(string connectionString)
            where TDbConnection : IDbConnection, new()
        {
            await ExecuteSqlStatementAsync<TDbConnection>(connectionString, DROP_DATABASE_SQL_STATEMENT);
        }

        protected virtual async Task ExecuteSqlStatementAsync<TDbConnection>(string connectionString, string sqlStatement)
            where TDbConnection : IDbConnection, new()
        {
            using var connection = new TDbConnection
            {
                ConnectionString = connectionString
            };

            await connection.ExecuteAsync(sqlStatement);
        }
    }
}