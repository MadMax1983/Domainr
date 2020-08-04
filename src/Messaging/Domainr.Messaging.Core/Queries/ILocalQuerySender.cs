using System.Threading;
using System.Threading.Tasks;

namespace Domainr.Messaging.Core.Queries
{
    public interface ILocalQuerySender
    {
        Task<TResult> QueryAsync<TResult, TQuery>(TQuery query, CancellationToken cancellationToken = default)
            where TQuery : IQuery<TResult>;
    }
}