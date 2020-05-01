namespace Domainr.EventStore.Data
{
    public interface ISqlStatementsLoader
    {
        string this[string key] { get; }
    }
}