using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Domainr.Core.EventSourcing.Abstraction;
using Domainr.Core.Infrastructure;
using Domainr.EventStore.Sql.Configuration;
using Domainr.EventStore.Sql.Data;
using Domainr.EventStore.Sql.Infrastructure;
using Domainr.EventStore.Sql.Models;
using Microsoft.Extensions.Logging;

namespace Domainr.EventStore.Sql
{
    public abstract class SqlEventStore<TSerializationType>
        : IEventStore,
          IEventStoreInitializer
    {
        private readonly ILogger<SqlEventStore<TSerializationType>> _logger;

        private readonly EventStoreSettings _settings;

        private readonly ISqlStatementsLoader _sqlStatementsLoader;

        private readonly IEventDataSerializer<TSerializationType> _eventDataSerializer;

        protected SqlEventStore(
            ILogger<SqlEventStore<TSerializationType>> logger,
            EventStoreSettings settings,
            ISqlStatementsLoader sqlStatementsLoader,
            IEventDataSerializer<TSerializationType> eventDataSerializer)
        {
            _logger = logger;

            _settings = settings;

            _sqlStatementsLoader = sqlStatementsLoader;

            _eventDataSerializer = eventDataSerializer;
        }

        public virtual async Task<IReadOnlyCollection<Event>> GetByAggregateRootIdAsync(string aggregateRootId, long fromVersion = Constants.INITIAL_VERSION)
        {
            return await ExecuteSqlStatement(async (connection, transaction) =>
            {
                var @params = new DynamicParameters();

                @params.Add("@AggregateRootId", aggregateRootId);
                @params.Add("@FromVersion", fromVersion);

                var sql = _sqlStatementsLoader[nameof(GetByAggregateRootIdAsync)];

                var eventEntities =
                    await connection.QueryAsync<EventEntity<TSerializationType>>(sql, @params, transaction);

                var events = eventEntities
                    .Select(eventEntity => _eventDataSerializer.Deserialize(eventEntity.Data, eventEntity.Type))
                    .ToList();

                return events;
            });
        }

        public virtual async Task SaveAsync(IReadOnlyCollection<Event> events)
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
                    @params.Add("@Data", _eventDataSerializer.Serialize(@event));

                    result += await connection.ExecuteAsync(_sqlStatementsLoader[nameof(SaveAsync)], @params, transaction);
                }

                return result;
            });
        }

        public virtual Task InitializeAsync()
        {
            return ExecuteSqlStatement(async (connection, transaction) =>
                await connection.ExecuteAsync(_sqlStatementsLoader[nameof(InitializeAsync)], transaction: transaction));
        }

        protected virtual async Task<TResult> ExecuteSqlStatement<TResult>(Func<IDbConnection, IDbTransaction, Task<TResult>> sqlFuncAsync)
        {
            var connection = CreateConnection(_settings.ConnectionStrings["EventStore"]);
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

        protected abstract IDbConnection CreateConnection(string connectionString);
    }
}