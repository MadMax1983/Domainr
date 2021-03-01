using System;
using System.IO;
using System.Runtime.Serialization;
using Domainr.Core.EventSourcing.Abstraction;
using Newtonsoft.Json.Serialization;

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

        public virtual Event Deserialize(byte[] byteArray, string eventType)
        {
            if (byteArray == null)
            {
                throw new ArgumentNullException(nameof(byteArray));
            }

            if (byteArray.Length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(byteArray));
            }

            if (string.IsNullOrWhiteSpace(eventType))
            {
                throw new ArgumentNullException(nameof(eventType));
            }

            using (var memoryStream = new MemoryStream(byteArray, 0, byteArray.Length))
            {
                memoryStream.Seek(0, SeekOrigin.Begin);

                var type = Type.GetType(eventType);
                if (type == null)
                {
                    throw new InvalidOperationException("Provided event type is invalid");
                }

                _binaryFormatter.Binder = new DefaultSerializationBinder();

                _binaryFormatter.Binder.BindToType(type.Assembly.FullName, type.FullName);

                var @event = (Event)_binaryFormatter.Deserialize(memoryStream);

                return @event;
            }
        }
    }
}