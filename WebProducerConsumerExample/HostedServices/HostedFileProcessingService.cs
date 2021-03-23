using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WebProducerConsumerExample.HostedServices
{
    public class HostedFileProcessingService : BackgroundService
    {
        private readonly IFileConsumerManager _manager;
        private readonly ILogger<HostedFileProcessingService> _logger;

        public HostedFileProcessingService(IFileConsumerManager manager, ILogger<HostedFileProcessingService> logger)
        {
            _manager = manager;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var logPulse = DateTime.UtcNow;
            while (!stoppingToken.IsCancellationRequested)
            {
                await _manager.EnsureConsumersRunning(stoppingToken);
                if (logPulse.Add(TimeSpan.FromMinutes(3)) < DateTime.UtcNow)
                {
                    _logger.LogInformation("Hosted file processing service is running");
                    logPulse = DateTime.UtcNow;
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

            }
        }
    }
}
