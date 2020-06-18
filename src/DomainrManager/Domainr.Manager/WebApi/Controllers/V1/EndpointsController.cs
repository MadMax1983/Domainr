using System.Linq;
using System.Threading.Tasks;
using Domainr.Manager.Core.Data;
using Domainr.Manager.WebApi.Contracts.V1.Requests;
using Domainr.Manager.WebApi.Contracts.V1.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Domainr.Manager.WebApi.Controllers.V1
{
    [Route("api/v1/endpoints")]
    public sealed class EndpointsController
        : ControllerBase
    {
        private readonly IEndpointRepository _endpointRepository;

        public EndpointsController(IEndpointRepository endpointRepository)
        {
            _endpointRepository = endpointRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] GetFilteredRequest request)
        {
            var endpoints = (await _endpointRepository.GetAsync(request.ItemsPerPage, request.Page, request.OrderBy, request.OrderType))
                .Select(endpoint => new GetFilteredResponse(endpoint.Id, endpoint.Url));

            return Ok(endpoints);
        }
    }
}