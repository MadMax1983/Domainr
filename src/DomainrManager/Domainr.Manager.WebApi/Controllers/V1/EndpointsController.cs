using System.Threading.Tasks;
using Domainr.Manager.Core.Data;
using Domainr.Manager.WebApi.Contracts.V1.Requests.Endpoints;
using Microsoft.AspNetCore.Mvc;

namespace Domainr.Manager.WebApi.Controllers.V1
{
    [Route("api/v1/endpoints")]
    public partial class EndpointsController
        : ControllerBase
    {
        private readonly IEndpointRepository _endpointRepository;

        public EndpointsController(IEndpointRepository endpointRepository)
        {
            _endpointRepository = endpointRepository;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest request)
        {
            await _endpointRepository.AddEndpointAsync(
                request.Id,
                request.Url,
                request.AuthEndpointUrl,
                request.ClientKey,
                request.ClientSecret);

            return Accepted();
        }

        [HttpPut]
        public async Task<IActionResult> UnregisterAsync([FromBody] UnregisterRequest request)
        {
            await _endpointRepository.RemoveEndpointAsync(request.Id);

            return Accepted();
        }
    }
}