using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Equinor.ProCoSys.Preservation.WebApi.Synchronization
{
    public class TimedSynchronization : IHostedService, IDisposable
    {
        private readonly ILogger<TimedSynchronization> _logger;
        private readonly IOptionsMonitor<SynchronizationOptions> _options;
        private readonly IServiceProvider _services;
        private System.Timers.Timer _timer;

        public TimedSynchronization(
            ILogger<TimedSynchronization> logger,
            IOptionsMonitor<SynchronizationOptions> options,
            IServiceProvider services)
        {
            _logger = logger;
            _options = options;
            _services = services;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed synchronization is running");

            _timer = new System.Timers.Timer
            {
                Interval = _options.CurrentValue.Interval.TotalMilliseconds,
                AutoReset = false
            };
            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();

            return Task.CompletedTask;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!_options.CurrentValue.Enabled)
            {
                _logger.LogInformation("Timed work disabled");
                return;
            }

            _logger.LogInformation("Doing timed work");
            try
            {
                using var scope = _services.CreateScope();
                var syncService =
                    scope.ServiceProvider
                        .GetRequiredService<ISynchronizationService>();

                syncService.Synchronize(default).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error doing timed work");
            }
            finally
            {
                _timer.Start();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed synchronization is stopping");
            _timer?.Stop();
            return Task.CompletedTask;
        }

        public void Dispose() => _timer?.Dispose();
    }
}
