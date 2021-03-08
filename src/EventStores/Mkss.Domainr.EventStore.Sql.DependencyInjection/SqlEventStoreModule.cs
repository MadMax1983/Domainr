using Domainr.Core.EventSourcing.Abstraction;
using Domainr.EventStore.Sql;
using Domainr.EventStore.Sql.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Mkss.Domainr.EventStore.Sql.DependencyInjection
{
    public static class SqlEventStoreModule
    {
        public static IServiceCollection AddSqlEventStore<TEventDataSerializationType>(this IServiceCollection services)
        {
            services.AddSingleton<SqlEventStore<TEventDataSerializationType>>();
            
            services.AddSingleton<IEventStore, SqlEventStore<TEventDataSerializationType>>(serviceProvider => serviceProvider.GetRequiredService<SqlEventStore<TEventDataSerializationType>>());
            services.AddSingleton<IEventStoreInitializer, SqlEventStore<TEventDataSerializationType>>(serviceProvider => serviceProvider.GetRequiredService<SqlEventStore<TEventDataSerializationType>>());
            
            services.AddSingleton<ISqlStatementsLoader, SqlStatementsLoader>();
            
            return services;
        }

        public static IApplicationBuilder UseSqlEventStore(this IApplicationBuilder app)
        {
            var sqlStatementsLoader = app.ApplicationServices.GetRequiredService<ISqlStatementsLoader>();
            
            sqlStatementsLoader.LoadScripts();
            
            return app;
        }
    }
}