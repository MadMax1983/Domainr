namespace Domainr.Messaging.Core.Commands
{
    public interface ICommand
        : IMessage
    {
        string Id { get; }

        string CorrelationId { get; }
    }
}