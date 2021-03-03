using System;
using System.Collections.Generic;
using Domainr.EventStore.Serializers.Tests.Doubles;
using Domainr.EventStore.Sql.Serializers;
using FluentAssertions;

namespace Domainr.EventStore.Sql.Tests.UnitTests.Serializers
{
    public abstract class SerializerTest<TEventDataSerializationType>
    {
        private const string STRING_PROP_1_VALUE = "123";
        private const string STRING_PROP_2_VALUE = "456";

        private readonly IEventDataSerializer<TEventDataSerializationType> _eventSerializer;

        protected SerializerTest(IEventDataSerializer<TEventDataSerializationType> eventSerializer)
        {
            _eventSerializer = eventSerializer;
        }

        public void TestSerializer()
        {
            // Arrange
            var @event = new SerializationTestEvent(
                Guid.NewGuid().ToString(),
                STRING_PROP_1_VALUE,
                123,
                new List<string> { "1", "2", "3" },
                new List<TestEventObject>
                {
                    new TestEventObject("007"),
                    new TestEventObject("005")
                },
                STRING_PROP_2_VALUE);

            // Act
            var serializedEvent = _eventSerializer.Serialize(@event);

            var deserializedEvent = (SerializationTestEvent)_eventSerializer.Deserialize(serializedEvent, @event.GetType().AssemblyQualifiedName);

            // Assert
            deserializedEvent.AggregateRootId
                .Should()
                .Be(@event.AggregateRootId);

            deserializedEvent.Version
                .Should()
                .Be(@event.Version);

            deserializedEvent.IntProp
                .Should()
                .Be(@event.IntProp);

            deserializedEvent.StringProp
                .Should()
                .Be(@event.StringProp);

            deserializedEvent.StringProp1
                .Should()
                .Be(@event.StringProp1);

            deserializedEvent.ObjCollection
                .Should()
                .HaveCount(@event.ObjCollection.Count);

            deserializedEvent.ObjCollection
                .Should()
                .BeEquivalentTo(@event.ObjCollection);

            deserializedEvent.StringCollection
                .Should()
                .HaveCount(@event.StringCollection.Count);

            deserializedEvent.StringCollection
                .Should()
                .BeEquivalentTo(@event.StringCollection);
        }
    }
}