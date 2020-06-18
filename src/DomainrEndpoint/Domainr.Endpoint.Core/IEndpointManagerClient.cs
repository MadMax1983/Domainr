using System.Threading.Tasks;

namespace Domainr.Endpoint.Core
{
    public interface IEndpointManagerClient
    {
        Task RegisterAsync();

        Task UnregisterAsync();
    }
}