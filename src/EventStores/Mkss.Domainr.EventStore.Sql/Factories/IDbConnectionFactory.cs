using System.Data;

namespace Domainr.EventStore.Sql.Factories
{
    public interface IDbConnectionFactory
    {
        IDbConnection Create(string connectionString);
    }
}