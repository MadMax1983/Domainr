using System;
using System.Threading.Tasks;
using Domainr.Core.EventSourcing.Abstraction;
using Domainr.EventStore.Mongo.Model;
using Domainr.EventStore.Mongo.Tests.Doubles;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using NUnit.Framework;

namespace Domainr.EventStore.Mongo.Tests.IntegrationTests
{
    [TestFixture]
    public sealed class MongoEventStoreTests
    {
        private IMongoClient _mongoClient;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _mongoClient = new MongoClient("mongodb://localhost:27017");
        }

        [TearDown]
        public void TearDown()
        {
            var db = _mongoClient.GetDatabase("arch-doc-event-store");

            var collection = db.GetCollection<EventDocument>("Events");

            collection.DeleteMany(Builders<EventDocument>.Filter.Ne(x => x.Id, Guid.Empty));
        }

        [Test]
        public async Task GIVEN__WHEN__THEN_()
        {
            // Arrange
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            // Act
            var eventStore = new MongoEventStore(config, _mongoClient);

            var event1 = new UserCreated(Guid.NewGuid().ToString(), "User1", "user1@test-domainr.com");
            event1.SetVersion(0);

            var event2 = new UserCreated(Guid.NewGuid().ToString(), "User2", "user2@test-domainr.com");
            event2.SetVersion(0);


            var event3 = new EmailChanged(event1.AggregateRootId, "new-user1@test-domainr.com");
            event3.SetVersion(1);

            var events = new Event[]
            {
                event1,
                event2,
                event3
            };

            //Assert
           await eventStore.SaveAsync(events);

            var dbEvents = await eventStore.GetByAggregateRootIdAsync(event1.AggregateRootId, event1.Version - 1);
            //var dbEvents = await eventStore.GetByAggregateRootIdAsync("a85e452e-034b-11eb-8725-77832f2f0171", -1);
        }
    }
}
