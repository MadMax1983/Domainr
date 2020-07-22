using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Domainr.Core.EventSourcing.Abstraction;
using Domainr.EventStore.Serializers;
using Domainr.EventStore.Sql.Configuration;
using Domainr.EventStore.Sql.Data;
using Domainr.EventStore.Sql.Tests.Doubles;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Domainr.EventStore.Sql.Tests.IntegrationTests
{
    public abstract class SqlEventStoreTests<TSqlEventStore>
        where TSqlEventStore : SqlEventStore<string>
    {
        private const string CREATE_DATABASE_SQL_STATEMENT = "CREATE DATABASE [EventStoreTests]";

        private const string DROP_DATABASE_SQL_STATEMENT = "ALTER DATABASE [EventStoreTests] SET  SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [EventStoreTests]";

        private const string INITIALIZE_DATABASE_SQL_STATEMENT = "CREATE TABLE [Events] ([Id] nvarchar(64) NOT NULL, [Version] bigint NOT NULL, [AggregateRootId] nvarchar(64) NOT NULL, [Type] nvarchar(512) NOT NULL, [Data] nvarchar(2048) NOT NULL)";

        private const string SAVE_SQL_STATEMENT = "INSERT INTO [Events] ([Id], [AggregateRootId], [Version], [Type], [Data]) VALUES (@Id, @AggregateRootId, @Version, @Type, @Data)";

        private const string GET_BY_AGGREGATE_ROOT_ID_SQL_STATEMENT = "SELECT [Type], [Data] FROM [Events] WHERE [AggregateRootId] = @AggregateRootId AND [Version] > @FromVersion";

        protected Mock<ILogger<TSqlEventStore>> MockLogger { get; } = new Mock<ILogger<TSqlEventStore>>();

        protected EventStoreSettings Settings { get; } = new EventStoreSettings();

        protected Mock<ISqlStatementsLoader> MockSqlStatementsLoader { get; } = new Mock<ISqlStatementsLoader>();

        protected Mock<IEventDataSerializer<string>> MockEventDataSerializer { get; } = new Mock<IEventDataSerializer<string>>();

        protected virtual async Task OneTimeSetUpInternal(string initialConnectionString, string connectionString)
        {
            Settings.ConnectionStrings = new Dictionary<string, string>
            {
                {"EventStoreInitialize", initialConnectionString},
                {"EventStore", connectionString},
            };

            await CreateDatabaseAsync<SqlConnection>(Settings.ConnectionStrings["EventStoreInitialize"]);

            MockSqlStatementsLoader
                .Setup(m => m[It.IsAny<string>()])
                .Returns((string sqlName) =>
                {
                    if (sqlName.Equals("InitializeAsync"))
                    {
                        return INITIALIZE_DATABASE_SQL_STATEMENT;
                    }

                    return sqlName.Equals("SaveAsync")
                        ? SAVE_SQL_STATEMENT
                        : GET_BY_AGGREGATE_ROOT_ID_SQL_STATEMENT;
                });

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

        public async Task ExecuteTest()
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

            var eventStore = CreateSqlEventStore();

            // Act
            await eventStore.InitializeAsync();

            await eventStore.SaveAsync(events);

            var queriedEvents = await eventStore.GetByAggregateRootIdAsync("arid");

            // Assert
            queriedEvents
                .Should()
                .BeEquivalentTo(events);

        }

        protected abstract TSqlEventStore CreateSqlEventStore();

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
            using var connection = new TDbConnection { ConnectionString = connectionString };


            await connection.ExecuteAsync(sqlStatement);
        }
    }
}