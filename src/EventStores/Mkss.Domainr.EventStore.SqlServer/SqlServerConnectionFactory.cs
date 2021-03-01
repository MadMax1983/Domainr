using System.Data;
using System.Data.SqlClient;
using Domainr.EventStore.Sql.Factories;

namespace Domainr.EventStore
{
    public sealed class SqlServerConnectionFactory
        : IDbConnectionFactory
    {
        public IDbConnection Create(string connectionString)
        {
            return new SqlConnection(connectionString);
        }
    }
}