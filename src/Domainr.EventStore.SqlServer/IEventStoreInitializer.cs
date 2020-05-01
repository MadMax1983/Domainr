using System.Threading.Tasks;

namespace Domainr.EventStore
{
    public interface IEventStoreInitializer
    {
        Task InitializeAsync();
    }
}