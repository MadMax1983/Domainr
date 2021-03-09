using System.Threading.Tasks;
using Domainr.EventStore.Factories;
using Domainr.EventStore.Sql.Tests.IntegrationTests;
using NUnit.Framework;

namespace Domainr.EventStore.SqlServer.Tests.IntegrationTests
{
    [TestFixture]
    public sealed class SqlServerEventStoreTests
        : SqlEventStoreTests<SqlServerConnectionFactory>
    {
        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            await OneTimeSetUpInternal(
                "Server=.\\SQLEXPRESS; Trusted_Connection=True;",
                "Server=.\\SQLEXPRESS; Database=EventStoreTests; Trusted_Connection=True;");
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            await OneTimeTearDownInternal();
        }

        [Test]
        public async Task GIVEN_events_to_save_WHEN_executing_calling_store_methods_THEN_initializes_event_store_AND_saves_events_AND_gets_events_from_event_store()
        {
            var eventStore = new SqlServerEventStore<string>(Settings, SqlStatementsLoader, new SqlServerConnectionFactory(), MockEventDataSerializer.Object);
            
            await ExecuteTest(eventStore, eventStore);
        }

        [Test]
        public async Task GIVEN_events_to_save_with_existing_concurrency_WHEN_executing_calling_store_methods_THEN_throws_ConcurrencyException()
        {
            var eventStore = new SqlServerEventStore<string>(Settings, SqlStatementsLoader, new SqlServerConnectionFactory(), MockEventDataSerializer.Object);
            
            await ExecuteConcurrencyCheckTest(eventStore, eventStore);
        }
    }
}