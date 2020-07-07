using System;
using System.IO;
using System.Runtime.Serialization;
using Domainr.Core.EventSourcing.Abstraction;

namespace Domainr.EventStore.Serializers.ByteArray
{
    public class ByteArraySerializer
        : IEventDataSerializer<byte[]>
    {
        private readonly IFormatter _binaryFormatter;

        public ByteArraySerializer(IFormatter formatter)
        {
            _binaryFormatter = formatter;
        }


        public virtual byte[] Serialize<TEvent>(TEvent @event)
            where TEvent : Event
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            using (var memoryStream = new MemoryStream())
            {
                _binaryFormatter.Serialize(memoryStream, @event);

                var byteArray = memoryStream.ToArray();

                return byteArray;
            }
        }

        public virtual Event Deserialize(byte[] obj, string type)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (obj.Length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(obj));
            }

            using (var memoryStream = new MemoryStream(obj, 0, obj.Length))
            {
                memoryStream.Seek(0, SeekOrigin.Begin);

                var @event = (Event) _binaryFormatter.Deserialize(memoryStream);

                return @event;
            }
        }
    }
}