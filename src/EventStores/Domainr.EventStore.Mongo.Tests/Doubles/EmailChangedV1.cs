using MongoDB.Bson.Serialization.Attributes;

namespace Domainr.EventStore.Mongo.Tests.Doubles
{
    internal sealed class EmailChangedV1
        : BaseEvent
    {
        public EmailChangedV1()
        {
        }

        [BsonConstructor]
        public EmailChangedV1(string aggregateRootId, string newEmail)
            : base(aggregateRootId)
        {
            NewEmail = newEmail;
        }

        internal string NewEmail { get; }
    }
}