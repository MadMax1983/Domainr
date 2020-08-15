using MongoDB.Bson.Serialization.Attributes;

namespace Domainr.EventStore.Mongo.Tests.Doubles
{
    internal sealed class EmailChanged
        : BaseEvent
    {
        public EmailChanged()
        {
        }

        [BsonConstructor]
        public EmailChanged(string aggregateRootId, string newEmail)
            : base(aggregateRootId)
        {
            NewEmail = newEmail;
        }

        internal string NewEmail { get; }
    }
}