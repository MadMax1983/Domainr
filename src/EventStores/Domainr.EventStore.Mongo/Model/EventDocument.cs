using System;
using Domainr.Core.EventSourcing.Abstraction;
using MongoDB.Bson.Serialization.Attributes;

namespace Domainr.EventStore.Mongo.Model
{
    internal sealed class EventDocument
    {
        [BsonId]
        public Guid Id { get; set; }

        public Event Data { get; set; }
    }
}