using System.Runtime.Serialization.Formatters.Binary;
using Domainr.EventStore.Serializers.ByteArray;
using NUnit.Framework;

namespace Domainr.EventStore.Serializers.Tests.UnitTests
{
    [TestFixture]
    public sealed class ByteArraySerializerTests
        : SerializerTest<byte[]>
    {
        public ByteArraySerializerTests()
            : base(new ByteArraySerializer(new BinaryFormatter()))
        {
        }

        [Test]
        public void GIVEN_event_WHEN_serializing_AND_deserializing_THEN_original_AND_serialized_event_data_are_equal()
        {
            TestSerializer();
        }
    }
}