using MongoDB.Bson.Serialization.Attributes;

namespace Domainr.EventStore.Mongo.Tests.Doubles
{
    internal sealed class UserCreated
        : BaseEvent
    {
        public UserCreated()
        {
        }

        [BsonConstructor]
        public UserCreated(string aggregateRootId, string username, string email)
            : base(aggregateRootId)
        {
            Username = username;

            Email = email;
        }

        internal string Username { get; }

        internal string Email { get; }
    }
}