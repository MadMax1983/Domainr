using Domainr.Endpoint.Core;
using Microsoft.Extensions.Hosting;

namespace Domainr.Endpoint.Service
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args)
                .Build()
                .Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSystemd()
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddEndpointManagerClient(options =>
                    {
                        options.Name = "TestEndpoint";
                        options.Description = "Endpoint description";
                        options.MicroserviceName = "MicroserviceName";

                        options.EndpointManagerUrl = "https://localhost:5001/";
                        options.EndpointManagerKey = "Key";
                        options.EndpointManagerSecret = "Secret";

                    });
                });
    }
}