using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domainr.Manager.Core.Data;
using Domainr.Manager.Core.Data.Models;

namespace Domainr.Manager.WebApi.Data.Repositories
{
    internal sealed class InMemoryEndpointRepository
        : IEndpointRepository
    {
        private readonly IDictionary<string, Endpoint> _endpoints = new Dictionary<string, Endpoint>();

        public Task<IReadOnlyCollection<Endpoint>> GetAsync(int itemsPerPage = 5, int page = 1, string orderBy = "url", string orderType = "asc")
        {
            var skippedItemsCount = (page - 1) * itemsPerPage;

            var endpoints = _endpoints.Values
                .Take(itemsPerPage)
                .Skip(skippedItemsCount)
                .OrderBy(x => orderBy)
                .ToList();

            return Task.FromResult<IReadOnlyCollection<Endpoint>>(endpoints);
        }


        public Task<bool> CheckIfExistsAsync(string endpointUrl)
        {
            var endpointExists = _endpoints.Any(endpoint => endpoint.Key.Equals(endpointUrl, StringComparison.CurrentCultureIgnoreCase));

            return Task.FromResult(endpointExists);
        }

        public Task AddEndpointAsync(string url, string authEndpointUrl, string clientKey, string clientSecret)
        {
            var endpoint = new Endpoint
            {
                Id = Guid.NewGuid(),
                Url = url,
                AuthEndpointUrl = authEndpointUrl,
                ClientKey = clientKey,
                ClientSecret = clientSecret
            };

            _endpoints.Add(url, endpoint);

            return Task.CompletedTask;
        }
    }
}