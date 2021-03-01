namespace Domainr.EventStore.Sql.Data
{
    public interface ISqlStatementsLoader
    {
        void LoadScripts();
        
        string this[string key] { get; }
    }
}