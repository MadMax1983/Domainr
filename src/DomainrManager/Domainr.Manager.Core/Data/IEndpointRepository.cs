using System.Collections.Generic;
using System.Threading.Tasks;
using Domainr.Manager.Core.Data.Models;

namespace Domainr.Manager.Core.Data
{
    public interface IEndpointRepository
    {
        Task<IReadOnlyCollection<Endpoint>> GetAsync(int itemsPerPage, int page, string orderBy, string orderType);

        Task<bool> CheckIfExistsAsync(string endpointUrl);

        Task AddEndpointAsync(string id, string url, string authEndpointUrl, string clientKey, string clientSecret);

        Task RemoveEndpointAsync(string id);
    }
}