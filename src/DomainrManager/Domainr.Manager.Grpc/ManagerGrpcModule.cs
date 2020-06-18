using Microsoft.Extensions.DependencyInjection;

namespace Domainr.Manager.Grpc
{
    public static class ManagerGrpcModule
    {
        public static IServiceCollection AddGrpcServices(this IServiceCollection services)
        {
            return services;
        }
    }
}