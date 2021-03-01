using System.Data;
using Domainr.EventStore.Sql.Factories;
using Npgsql;

namespace Domainr.EventStore.Postgresql
{
    public sealed class NpgConnectionFactory
        : IDbConnectionFactory
    {
        public IDbConnection Create(string connectionString)
        {
            return new NpgsqlConnection(connectionString);
        }
    }
}