using Domainr.Core.EventSourcing.Abstraction;
using Domainr.EventStore.Sql;
using Domainr.EventStore.Sql.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mkss.Domainr.EventStore.Sql.DependencyInjection
{
    public static class SqlEventStoreModule
    {
        public static IServiceCollection AddSqlEventStore(this IServiceCollection services)
        {
            services.TryAddSingleton<IEventStore, SqlEventStore<string>>();
            services.TryAddSingleton<IEventStoreInitializer, SqlEventStore<string>>();
            
            services.TryAddSingleton<ISqlStatementsLoader, SqlStatementsLoader>();
            
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