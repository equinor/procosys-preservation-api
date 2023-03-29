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
        private string _machine;

        public TimedSynchronization(
            ILogger<TimedSynchronization> logger,
            IOptionsMonitor<SynchronizationOptions> options,
            IServiceProvider services)
        {
            _logger = logger;
            _options = options;
            _services = services;
            _machine = Environment.MachineName;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // THIS CODE WAS WRITTEN TO RUN A ONETIME TRANSFORMATION WHEN WE INTRODUCED ProCoSysGuid
            // WE KEEP THE CODE ... MAYBE WE WANT TO DO SIMILAR STUFF LATER

            //_timer = new System.Timers.Timer
            //{
            //    Interval = _options.CurrentValue.Interval.TotalMilliseconds,
            //    AutoReset = false
            //};
            //_timer.Elapsed += Timer_Elapsed;
            //_timer.Start();
            //_logger.LogInformation($"Timed work configured on {_machine}. Interval = {_options.CurrentValue.Interval}");

            return Task.CompletedTask;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                _logger.LogInformation($"Timed work starting on {_machine}");
                using var scope = _services.CreateScope();
                var syncService =
                    scope.ServiceProvider
                        .GetRequiredService<ISynchronizationService>();

                syncService.Synchronize(default).GetAwaiter().GetResult();
                _logger.LogInformation($"Timed work finished on {_machine}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Timed work error on {_machine}");
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
