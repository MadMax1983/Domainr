using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domainr.EventStore.Sql.Configuration;
using Domainr.EventStore.Sql.Data;
using Domainr.EventStore.Sql.Tests.Doubles;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Domainr.EventStore.Sql.Tests.IntegrationTests
{
    [TestFixture]
    public sealed class SqlEventStoreTests
    {
        private readonly Mock<ILogger<SqliteEventStore<string>>> _mockLogger = new Mock<ILogger<SqliteEventStore<string>>>();

        private readonly Mock<ISqlStatementsLoader> _mockSqlStatementsLoader = new Mock<ISqlStatementsLoader>();

        private readonly EventStoreSettings _settings = new EventStoreSettings
        {
            ConnectionStrings = new Dictionary<string, string>
            {
                { "EventStore", "Data Source=:memory:" }
            }
        };

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var jsonString = "{ Id: \"_id\", Version: \"10\" }";

            var @event = (TestEvent)JsonConvert.DeserializeObject(jsonString,  Type.GetType("Domainr.EventStore.Sql.Tests.Doubles.TestEvent"));

            _mockSqlStatementsLoader
                .Setup(m => m[It.IsAny<string>()])
                .Returns("SELECT '_id' AS [Id], 10 AS [Version], 'AggregateRootId' AS [AggregateRootId] 'arid' AS [AggregateRootId], '{ Id: \"_id\", Version: \"10\" }' AS [Data]");
        }

        [Test]
        public async Task GIVEN__WHEN__THEN_()
        {
            // Arrange
            var eventStore = new SqliteEventStore<string>(_mockLogger.Object, _settings, _mockSqlStatementsLoader.Object, null);

            // Act
            await eventStore.GetByAggregateRootIdAsync("");

            // Assert

        }
    }
}