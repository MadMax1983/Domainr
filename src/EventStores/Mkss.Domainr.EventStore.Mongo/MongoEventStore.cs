using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domainr.Core.EventSourcing.Abstraction;
using Domainr.EventStore.Mongo.Conventions;
using Domainr.EventStore.Mongo.Model;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace Domainr.EventStore.Mongo
{
    public sealed class MongoEventStore
        : IEventStore
    {
        private const string EVENT_COLLECTION_NAME = "Events";

        private readonly string _databaseName;

        private readonly IMongoClient _mongoClient;

        public MongoEventStore(IConfiguration configuration, IMongoClient mongoClient)
        {
            _databaseName = configuration["Mongo:DatabaseName"] ?? "EventStore";

            var conventionPack = new ConventionPack
            {
                new MapReadOnlyPropertiesConvention()
            };

            ConventionRegistry.Register("Conventions", conventionPack, _ => true);

            _mongoClient = mongoClient;
        }

        public async Task<IReadOnlyCollection<Event>> GetByAggregateRootIdAsync(string aggregateRootId, long fromVersion, CancellationToken cancellationToken = default)
        {
            var database = _mongoClient.GetDatabase(_databaseName);

            var collection = database.GetCollection<EventDocument>(EVENT_COLLECTION_NAME);

            var filtersBuilder = Builders<EventDocument>.Filter;

            var filters = filtersBuilder.And(
                filtersBuilder.Eq(doc => doc.Data.AggregateRootId, aggregateRootId),
                filtersBuilder.Gt(doc => doc.Data.Version, fromVersion)
            );

            var events = await collection.FindAsync(filters, cancellationToken: cancellationToken);

            return events
                .ToList()
                .Select(eventDocument => eventDocument.Data)
                .ToList();
        }

        public async Task SaveAsync(IReadOnlyCollection<Event> events, CancellationToken cancellationToken = default)
        {
            var database = _mongoClient.GetDatabase(_databaseName);

            var collection = database.GetCollection<EventDocument>(EVENT_COLLECTION_NAME);

            var documents = events
                .Select(@event => new EventDocument
                {
                    Data = @event
                });

            await collection.InsertManyAsync(documents, cancellationToken: cancellationToken);
        }
    }
}