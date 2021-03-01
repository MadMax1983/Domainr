using System.Threading.Tasks;
using Domainr.Core.Infrastructure;
using Domainr.EventStore.InMemory.Tests.Doubles;
using FluentAssertions;
using NUnit.Framework;

namespace Domainr.EventStore.InMemory.Tests.IntegrationTests
{
    [TestFixture]
    public sealed class InMemoryEventStoreTests
    {
        [Test]
        public async Task GIVEN_event_store_writing_instances_WHEN_saving_events_THEN_retrieves_events_with_event_store_read_instance()
        {
            // Arrange
            var eventStoreWriter = new InMemoryEventStore();
            var eventStoreReader = new InMemoryEventStore();

            var event1 = new TestEvent("1", "Prop1_1");

            event1.SetVersion(0);

            var event2 = new TestEvent("1", "Prop1_2");

            event2.SetVersion(1);

            var event3 = new TestEvent("2", "Prop1_3");

            event3.SetVersion(0);

            // Act
            await eventStoreWriter.SaveAsync(new []{ event2 });
            await eventStoreWriter.SaveAsync(new[] { event1 });
            await eventStoreWriter.SaveAsync(new []{ event3 });

            var events = await eventStoreReader.GetByAggregateRootIdAsync("1", Constants.INITIAL_VERSION);

            // Assert
            events
                .Should()
                .HaveCount(2);

            events
                .Should()
                .BeInAscendingOrder(e => e.Version);
        }
    }
}