using System.Data;
using Domainr.EventStore.Sql.Factories;
using Oracle.ManagedDataAccess.Client;

namespace Domainr.EventStore.Oracle
{
    public sealed class OracleConnectionFactory
        : IDbConnectionFactory
    {
        public IDbConnection Create(string connectionString)
        {
            return new OracleConnection(connectionString);
        }
    }
}