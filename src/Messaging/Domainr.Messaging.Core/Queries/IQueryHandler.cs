using System.Threading;
using System.Threading.Tasks;

namespace Domainr.Messaging.Core.Queries
{
    public interface IQueryHandler<in TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        Task<TResult> ExecuteAsync(TQuery query, CancellationToken cancellationToken = default);
    }
}