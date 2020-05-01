using System;
using System.Collections.Generic;
using Domainr.EventStore.Configuration;
using Domainr.EventStore.Data;

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

            var eventStore = new SqlServerEventStore(null, appSettings, filesLoader);

            eventStore.InitializeAsync().Wait();

            var events = eventStore.GetByAggregateRootIdAsync("123").Result;

            Console.WriteLine("Hello World!");
        }
    }
}