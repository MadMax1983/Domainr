using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Domainr.Core.EventSourcing.Abstraction;
using Domainr.Core.Infrastructure;
using Domainr.EventStore.Serializers;
using Domainr.EventStore.Sql.Configuration;
using Domainr.EventStore.Sql.Data;
using Domainr.EventStore.Sql.Factories;
using Domainr.EventStore.Sql.Models;

namespace Domainr.EventStore.Sql
{
    public class SqlEventStore<TSerializationType>
        : IEventStore,
          IEventStoreInitializer
    {
        private readonly string _connectionString;
        
        public SqlEventStore(
            EventStoreSettings settings,
            ISqlStatementsLoader sqlStatementsLoader,
            IDbConnectionFactory connectionFactory,
            IEventDataSerializer<TSerializationType> eventDataSerializer)
        {
            _connectionString = settings.ConnectionStrings["EventStore"];

            SqlStatementsLoader = sqlStatementsLoader;
            ConnectionFactory = connectionFactory;

            EventDataSerializer = eventDataSerializer;
        }

        protected ISqlStatementsLoader SqlStatementsLoader { get; }
        
        protected IDbConnectionFactory ConnectionFactory { get; }

        protected IEventDataSerializer<TSerializationType> EventDataSerializer { get; }

        public virtual async Task<IReadOnlyCollection<Event>> GetByAggregateRootIdAsync(string aggregateRootId, long fromVersion = Constants.INITIAL_VERSION, CancellationToken cancellationToken = default)
        {
            return await ExecuteSqlStatement(async (connection, transaction) =>
            {
                var inputParams = new
                {
                    AggregateRootId = aggregateRootId,
                    FromVersion = fromVersion
                };

                var sql = SqlStatementsLoader[nameof(GetByAggregateRootIdAsync)];

                var commandDefinition = new CommandDefinition(
                    sql,
                    inputParams,
                    transaction,
                    cancellationToken: cancellationToken);

                var eventEntities = await connection.QueryAsync<EventEntity<TSerializationType>>(commandDefinition);

                var events = eventEntities
                    .Select(eventEntity => EventDataSerializer.Deserialize(eventEntity.Data, eventEntity.Type))
                    .ToList();

                return events;
            });
        }

        public virtual async Task SaveAsync(IReadOnlyCollection<Event> events, CancellationToken cancellationToken = default)
        {
            await ExecuteSqlStatement(async (connection, transaction) =>
            {
                var result = 0;

                var sql = SqlStatementsLoader[nameof(SaveAsync)];

                foreach (var @event in events)
                {
                    var eventEntity = new
                    {
                        Id = Guid.NewGuid().ToString(),
                        Version = @event.Version,
                        AggregateRootId = @event.AggregateRootId,
                        Type = @event.GetType().AssemblyQualifiedName,
                        Data = EventDataSerializer.Serialize(@event)
                    };
                    
                    var commandDefinition = new CommandDefinition(
                        sql,
                        eventEntity,
                        transaction,
                        cancellationToken: cancellationToken);

                    result += await connection.ExecuteAsync(commandDefinition);
                }

                return result;
            });
        }

        public virtual Task InitializeAsync()
        {
            return ExecuteSqlStatement(async (connection, transaction) =>
            {
                var sql = SqlStatementsLoader[nameof(InitializeAsync)];
                
                var commandDefinition = new CommandDefinition(
                    sql,
                    transaction: transaction);
                
                return await connection.ExecuteAsync(commandDefinition);
            });
        }

        protected virtual async Task<TResult> ExecuteSqlStatement<TResult>(Func<IDbConnection, IDbTransaction, Task<TResult>> sqlFuncAsync)
        {
            var connection = ConnectionFactory.Create(_connectionString);
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
            catch
            {
                transaction.Rollback();

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