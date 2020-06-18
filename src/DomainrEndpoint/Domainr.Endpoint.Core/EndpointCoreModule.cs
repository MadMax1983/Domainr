using System;
using Microsoft.Extensions.DependencyInjection;

namespace Domainr.Endpoint.Core
{
    public static class EndpointCoreModule
    {
        public static IServiceCollection AddEndpointManagerClient(this IServiceCollection services, Action<EndpointManagerClientOptions> setupOptions)
        {
            services.AddHostedService<Worker>();

            var endpointOptions = new EndpointManagerClientOptions();

            setupOptions(endpointOptions);

            // TODO: Validate options
            // TODO: Check if it is valid URL
            services.AddSingleton(_ => endpointOptions);
            services.AddSingleton<IEndpointManagerClient, EndpointManagerClient>();

            return services;
        }
    }
}