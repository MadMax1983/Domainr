﻿using NUnit.Framework;

namespace Domainr.EventStore.Serializers.Tests.UnitTests
{
    [TestFixture]
    public sealed class JsonEventDataSerializerTests
        : SerializerTest<string>
    {
        public JsonEventDataSerializerTests()
            : base(new JsonEventDataSerializer())
        {
            
        }

        [Test]
        public void GIVEN_event_WHEN_serializing_AND_deserializing_THEN_original_AND_serialized_event_data_are_equal()
        {
            TestSerializer();
        }
    }
}