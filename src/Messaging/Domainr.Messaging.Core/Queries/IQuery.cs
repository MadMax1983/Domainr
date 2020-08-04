namespace Domainr.Messaging.Core.Queries
{
    public interface IQuery<out TResult>
        : IMessage
    {
    }
}