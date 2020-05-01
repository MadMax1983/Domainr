using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Dapper;
using Domainr.Core.EventSourcing.Abstraction;
using Domainr.Core.Infrastructure;
using Domainr.EventStore.Configuration;
using Domainr.EventStore.Data;
using Domainr.EventStore.Models;
using Microsoft.Extensions.Logging;

namespace Domainr.EventStore
{
    public sealed class SqlServerEventStore
        : IEventStore,
            IEventStoreInitializer
    {
        private readonly ILogger<SqlServerEventStore> _logger;

        private readonly EventStoreSettings _appSettings;

        private readonly ISqlStatementsLoader _sqlStatementsLoader;

        public SqlServerEventStore(ILogger<SqlServerEventStore> logger, EventStoreSettings appSettings,
            ISqlStatementsLoader sqlStatementsLoader)
        {
            _logger = logger;

            _appSettings = appSettings;

            _sqlStatementsLoader = sqlStatementsLoader;
        }

        public async Task<IReadOnlyCollection<Event>> GetByAggregateRootIdAsync(string aggregateRootId, long fromVersion = Constants.INITIAL_VERSION)
        {
            return await ExecuteSqlStatement(async (connection, transaction) =>
            {
                var @params = new DynamicParameters();

                @params.Add("@AggregateRootId", aggregateRootId);
                @params.Add("@FromVersion", fromVersion);

                var eventEntities =
                    await connection.QueryAsync<EventEntity>(_sqlStatementsLoader[nameof(GetByAggregateRootIdAsync)],
                        @params, transaction);

                var events = eventEntities
                    .Select(eventEntity => JsonSerializer.Deserialize<Event>(eventEntity.Data))
                    .ToList();

                return events;
            });
        }

        public async Task SaveAsync(IReadOnlyCollection<Event> events)
        {
            await ExecuteSqlStatement(async (connection, transaction) =>
            {
                var result = 0;

                foreach (var @event in events)
                {
                    var @params = new DynamicParameters();

                    @params.Add("@Id", Uuid.Create().ToString());
                    @params.Add("@Version", @event.Version);
                    @params.Add("@AggregateRootId", @event.AggregateRootId);
                    @params.Add("@Data", JsonSerializer.Serialize(@event));

                    result += await connection.ExecuteAsync(_sqlStatementsLoader[nameof(SaveAsync)], @params,
                        transaction);
                }

                return result;
            });
        }

        public async Task InitializeAsync()
        {
            await ExecuteSqlStatement(async (connection, transaction) =>
                await connection.ExecuteAsync(_sqlStatementsLoader[nameof(InitializeAsync)], transaction: transaction));
        }

        private async Task<TResult> ExecuteSqlStatement<TResult>(Func<IDbConnection, IDbTransaction, Task<TResult>> sqlFuncAsync)
        {
            var connection = new SqlConnection(_appSettings.ConnectionStrings["EventStore"]);
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            var transaction = connection.BeginTransaction();

            try
            {
                var result = await sqlFuncAsync(connection, transaction);

                transaction.Commit();

                return result;
            }
            catch (Exception ex)
            {
                transaction.Rollback();

                _logger.LogError(ex.Message, ex);

                throw;
            }
            finally
            {
                transaction.Dispose();
                connection.Dispose();
            }
        }
    }
}