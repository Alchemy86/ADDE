using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cascade.DirectoryAgent
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                _logger.LogDebug("Worker running with more info {MachineName}", System.Environment.MachineName);
                _logger.LogTrace("Worker running with VERY critical information {UserName}", System.Environment.UserName);

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
