using System.Threading.Tasks;
using Domainr.Manager.Core.Data;
using Grpc.Core;

namespace Domainr.Manager.Grpc.Services
{
    public sealed class EndpointsService
        : Endpoints.EndpointsBase
    {
        private readonly IEndpointRepository _endpointRepository;

        public EndpointsService(IEndpointRepository endpointRepository)
        {
            _endpointRepository = endpointRepository;
        }

        public override async Task<AddEndpointResponse> AddEndpoint(AddEndpointRequest request, ServerCallContext context)
        {
            if (!(await _endpointRepository.CheckIfExistsAsync(request.Url)))
            {
                await _endpointRepository.AddEndpointAsync(
                    request.Id,
                    request.Url,
                    request.AuthEndpointUrl,
                    request.ClientKey,
                    request.ClientSecret);
            }

            return new AddEndpointResponse();
        }
    }
}