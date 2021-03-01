using System.Data;
using Domainr.EventStore.Sql.Factories;
using MySql.Data.MySqlClient;

namespace Domainr.EventStore.MySql
{
    public sealed class MySqlConnectionFactory
        : IDbConnectionFactory
    {
        public IDbConnection Create(string connectionString)
        {
            return new MySqlConnection(connectionString);
        }
    }
}