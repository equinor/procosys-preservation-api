using System;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Command.PersonCommands.CreatePerson;
using Equinor.ProCoSys.Preservation.WebApi.Authentication;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Equinor.ProCoSys.Preservation.WebApi.Middleware
{
    /// <summary>
    /// Ensure that PreservationApiObjectId (i.e the application) exists as Person.
    /// Needed when application modifies data, setting ModifiedById for changed records
    /// </summary>
    public class VerifyApplicationExistsAsPerson : IHostedService
    {
        private readonly IServiceScopeFactory _serviceProvider;
        private readonly IOptionsMonitor<PreservationAuthenticatorOptions> _options;
        private readonly ILogger<VerifyApplicationExistsAsPerson> _logger;

        public VerifyApplicationExistsAsPerson(
            IServiceScopeFactory serviceProvider,
            IOptionsMonitor<PreservationAuthenticatorOptions> options, 
            ILogger<VerifyApplicationExistsAsPerson> logger)
        {
            _serviceProvider = serviceProvider;
            _options = options;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();

            var mediator =
                scope.ServiceProvider
                    .GetRequiredService<IMediator>();
            var currentUserSetter =
                scope.ServiceProvider
                    .GetRequiredService<ICurrentUserSetter>();

            var oid = _options.CurrentValue.PreservationApiObjectId;
            _logger.LogInformation($"Ensuring '{oid}' exists as Person");
            try
            {
                currentUserSetter.SetCurrentUserOid(oid);
                await mediator.Send(new CreatePersonCommand(oid), cancellationToken);
                _logger.LogInformation($"'{oid}' ensured");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception handling {nameof(CreatePersonCommand)}");
                throw;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
