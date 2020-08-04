using System;
using System.Threading;
using System.Threading.Tasks;
using Domainr.Messaging.Core.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Domainr.Messaging.Core.Queries
{
    public sealed class LocalQuerySender
        : ILocalQuerySender
    {
        private readonly ILogger<LocalQuerySender> _logger;

        private readonly IContainer _container;

        public LocalQuerySender(ILogger<LocalQuerySender> logger, IContainer container)
        {
            _logger = logger;

            _container = container;
        }

        public async Task<TResult> QueryAsync<TResult, TQuery>(TQuery query, CancellationToken cancellationToken = default)
            where TQuery : IQuery<TResult>
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var queryHandler = _container.GetService<IQueryHandler<TQuery, TResult>>();
            if (queryHandler == null)
            {
                throw new Exception(); // TODO: Add message and custom exception here.
            }

            try
            {
                return await queryHandler.ExecuteAsync(query, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                throw;
            }
        }
    }
}