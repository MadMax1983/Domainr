using System;
using System.Collections.Generic;
using Domainr.EventStore.Serializers;
using Domainr.EventStore.Sql;
using Domainr.EventStore.Sql.Configuration;
using Domainr.EventStore.Sql.Data;

namespace Domainr.EventStore.SqlServer.Cmd
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var appSettings = new EventStoreSettings
            {
                ConnectionStrings = new Dictionary<string, string>
                {
                    { "EventStore", "Server=\".\\SQLExpress\"; Database=\"EventStoreTest\"; Integrated Security=SSPI;" }
                }
            };

            var filesLoader = new SqlStatementsLoader();

            var connectionFactory = new SqlServerConnectionFactory();

            var eventDataSerializer = new JsonEventDataSerializer();

            var eventStore = new SqlEventStore<string>(
                appSettings,
                filesLoader,
                connectionFactory,
                eventDataSerializer);

            eventStore.InitializeAsync().Wait();

            var events = eventStore.GetByAggregateRootIdAsync("123").Result;

            Console.WriteLine("Hello World!");
        }
    }
}