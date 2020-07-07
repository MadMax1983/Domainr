using System.Threading.Tasks;

namespace Domainr.EventStore.Sql
{
    public interface IEventStoreInitializer
    {
        Task InitializeAsync();
    }
}