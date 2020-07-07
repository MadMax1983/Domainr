namespace Domainr.EventStore.Sql.Data
{
    public interface ISqlStatementsLoader
    {
        string this[string key] { get; }
    }
}