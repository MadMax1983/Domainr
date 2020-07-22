using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Domainr.Core.EventSourcing.Abstraction;
using Domainr.Core.Infrastructure;
using Domainr.EventStore.Serializers;
using Domainr.EventStore.Sql.Configuration;
using Domainr.EventStore.Sql.Data;
using Domainr.EventStore.Sql.Models;
using Microsoft.Extensions.Logging;

namespace Domainr.EventStore.Sql
{
    public abstract class SqlEventStore<TSerializationType>
        : IEventStore,
          IEventStoreInitializer
    {
        protected SqlEventStore(
            ILogger<SqlEventStore<TSerializationType>> logger,
            EventStoreSettings settings,
            ISqlStatementsLoader sqlStatementsLoader,
            IEventDataSerializer<TSerializationType> eventDataSerializer)
        {
            Logger = logger;

            Settings = settings;

            SqlStatementsLoader = sqlStatementsLoader;

            EventDataSerializer = eventDataSerializer;
        }

        public ILogger<SqlEventStore<TSerializationType>> Logger { get; }

        public EventStoreSettings Settings { get; }

        public ISqlStatementsLoader SqlStatementsLoader { get; }

        public IEventDataSerializer<TSerializationType> EventDataSerializer { get; }

        public virtual async Task<IReadOnlyCollection<Event>> GetByAggregateRootIdAsync(string aggregateRootId, long fromVersion = Constants.INITIAL_VERSION)
        {
            return await ExecuteSqlStatement(async (connection, transaction) =>
            {
                var @params = new DynamicParameters();

                @params.Add("@AggregateRootId", aggregateRootId);
                @params.Add("@FromVersion", fromVersion);

                var sql = SqlStatementsLoader[nameof(GetByAggregateRootIdAsync)];

                var eventEntities =
                    await connection.QueryAsync<EventEntity<TSerializationType>>(sql, @params, transaction);

                var events = eventEntities
                    .Select(eventEntity => EventDataSerializer.Deserialize(eventEntity.Data, eventEntity.Type))
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
                    @params.Add("@Type", @event.GetType().AssemblyQualifiedName);
                    @params.Add("@Data", EventDataSerializer.Serialize(@event));

                    result += await connection.ExecuteAsync(SqlStatementsLoader[nameof(SaveAsync)], @params, transaction);
                }

                return result;
            });
        }

        public virtual Task InitializeAsync()
        {
            return ExecuteSqlStatement(async (connection, transaction) => await connection.ExecuteAsync(SqlStatementsLoader[nameof(InitializeAsync)], transaction: transaction));
        }

        protected virtual async Task<TResult> ExecuteSqlStatement<TResult>(Func<DbConnection, IDbTransaction, Task<TResult>> sqlFuncAsync)
        {
            var connection = CreateConnection(Settings.ConnectionStrings["EventStore"]);
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                var result = await sqlFuncAsync(connection, transaction);

                transaction.Commit();

                return result;
            }
            catch (Exception ex)
            {
                transaction.Rollback();

                Logger.LogError(ex.Message, ex);

                throw;
            }
            finally
            {
                transaction.Dispose();
                connection.Dispose();
            }
        }

        protected abstract DbConnection CreateConnection(string connectionString);
    }
}