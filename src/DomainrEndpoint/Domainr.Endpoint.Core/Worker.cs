using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Domainr.Endpoint.Core
{
    public sealed class Worker
        : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        private readonly IEndpointManagerClient _endpointManagerClient;

        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;

            _endpointManagerClient = (IEndpointManagerClient) serviceProvider.GetService(typeof(IEndpointManagerClient));
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            // TODO: Send register request to Endpoint Manager if it is configured.
            // TODO: Setup gRPC, or endpoint for events

            await _endpointManagerClient.RegisterAsync();

            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                await Task.Delay(1000, stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _endpointManagerClient.UnregisterAsync();

            await base.StopAsync(cancellationToken);
        }
    }
}