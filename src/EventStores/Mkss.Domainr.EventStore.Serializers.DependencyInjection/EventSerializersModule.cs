using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Domainr.EventStore.Serializers;
using Domainr.EventStore.Serializers.ByteArray;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mkss.Domainr.EventStore.Serializers.DependencyInjection
{
    public static class EventSerializersModule
    {
        public static IServiceCollection AddJsonEventSerializer(this IServiceCollection services)
        {
            services.TryAddSingleton<IEventDataSerializer<string>, JsonEventDataSerializer>();

            return services;
        }

        public static IServiceCollection AddByteArrayEventSerializer(this IServiceCollection services)
        {
            services.TryAddSingleton<IFormatter, BinaryFormatter>();

            services.TryAddSingleton<IEventDataSerializer<byte[]>, ByteArraySerializer>();
            
            return services;
        }
    }
}