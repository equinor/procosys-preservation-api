using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Equinor.Procosys.Preservation.WebApi.Synchronization
{
    public class TimedSynchronization : IHostedService, IDisposable
    {
        private readonly ILogger<TimedSynchronization> _logger;
        private readonly IOptionsMonitor<SynchronizationOptions> _options;
        private Timer _timer;

        public TimedSynchronization(ILogger<TimedSynchronization> logger, IOptionsMonitor<SynchronizationOptions> options)
        {
            _logger = logger;
            _options = options;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed synchronization is running");

            _timer = new Timer(DoWork, null, _options.CurrentValue.StartupDelay, _options.CurrentValue.Interval);

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            _logger.LogInformation("Doing timed work");

            // Change the timer period to a great number to avoid re-triggering before the work is complete
            _timer.Change(TimeSpan.FromHours(6), TimeSpan.FromHours(6));

            // The work is done - change the timer period back to the intended period
            _timer.Change(_options.CurrentValue.Interval, _options.CurrentValue.Interval);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed synchronization is stopping");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose() => _timer?.Dispose();
    }
}
