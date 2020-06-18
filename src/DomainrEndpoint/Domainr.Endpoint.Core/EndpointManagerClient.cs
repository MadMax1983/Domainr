using System.Threading.Tasks;
using Domainr.Manager.Grpc;
using Grpc.Net.Client;

namespace Domainr.Endpoint.Core
{
    internal sealed class EndpointManagerClient
        : IEndpointManagerClient
    {
        private readonly EndpointManagerClientOptions _options;

        public EndpointManagerClient(EndpointManagerClientOptions options)
        {
            _options = options;
        }


        public async Task RegisterAsync()
        {
            using var managerChannel = GrpcChannel.ForAddress(_options.EndpointManagerUrl);

            var endpointsClient = new Endpoints.EndpointsClient(managerChannel);

            var endpoint = new AddEndpointRequest
            {
                Url = $"https://localhost:6001"
            };

            await endpointsClient.AddEndpointAsync(endpoint);
        }

        public Task UnregisterAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}