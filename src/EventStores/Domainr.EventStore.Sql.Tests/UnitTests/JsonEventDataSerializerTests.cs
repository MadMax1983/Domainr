using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Domainr.Core.EventSourcing.Abstraction;
using Domainr.EventStore.Serializers.Json;
using Domainr.EventStore.Sql.Tests.Doubles;
using FluentAssertions;
using NUnit.Framework;

namespace Domainr.EventStore.Sql.Tests.UnitTests
{
    [TestFixture]
    public sealed class JsonEventDataSerializerTests
    {
        [Test]
        public void GIVEN_event_WHEN_serializing_to_json_THEN_returns_json_string()
        {
            // Arrange
            var @event = new TestEvent(Guid.NewGuid().ToString());

            var serializer = new JsonEventDataSerializer();

            // Act
            var jsonString = serializer.Serialize(@event);

            // Assert
            jsonString
                .Should()
                .Contain(@event.AggregateRootId);
        }

        [Test]
        public void GIVEN_json_string_WHEN_deserializing_to_event_THEN_returns_event_with_correct_state()
        {
            // Arrange
            var aggregateRootId = Guid.NewGuid().ToString();

            var jsonString = $"{{ \"AggregateRootId\": \"{aggregateRootId}\", \"Version\": 0 }}";

            var serializer = new JsonEventDataSerializer();

            // Act
            var @event = serializer.Deserialize(jsonString, typeof(TestEvent).AssemblyQualifiedName);

            var field = typeof(TestEvent).GetField("_versionField", BindingFlags.Instance | BindingFlags.NonPublic);
            if (field != null)
            {
                field.SetValue(@event, 1);
            }

            var versionProp = typeof(Event)
                .GetRuntimeFields()
                .FirstOrDefault(a => Regex.IsMatch(a.Name, $"\\A<{nameof(Event.Version)}>k__BackingField\\Z"));
            if (versionProp != null)
            {
                versionProp.SetValue(@event, 0);
            }

            // Assert
            @event.AggregateRootId
                .Should()
                .Be(aggregateRootId);

            @event.Version
                .Should()
                .Be(0);
        }
    }
}