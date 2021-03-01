using System.Data;
using System.Data.SQLite;
using Domainr.EventStore.Sql.Factories;

namespace Domainr.EventStore.Sqlite
{
    public sealed class SqliteConnectionFactory
        : IDbConnectionFactory
    {
        public IDbConnection Create(string connectionString)
        {
            return new SQLiteConnection(connectionString);
        }
    }
}