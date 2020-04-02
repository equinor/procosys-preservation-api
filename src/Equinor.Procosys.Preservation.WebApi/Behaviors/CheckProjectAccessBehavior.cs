using System;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.WebApi.Misc;
using Equinor.Procosys.Preservation.WebApi.ProjectAccess;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Equinor.Procosys.Preservation.WebApi.Behaviors
{
    public class CheckProjectAccessBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
    {
        private readonly ILogger<CheckProjectAccessBehavior<TRequest, TResponse>> _logger;
        private readonly IProjectAccessValidator _projectAccess;
        public CheckProjectAccessBehavior(ILogger<CheckProjectAccessBehavior<TRequest, TResponse>> logger, IProjectAccessValidator projectAccess)
        {
            _logger = logger;
            _projectAccess = projectAccess;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var typeName = request.GetGenericTypeName();

            _logger.LogInformation($"----- Checking project access for {typeName}");

            if (!await _projectAccess.ValidateAsync(request))
            {
                _logger.LogWarning($"User do not have access to project - {typeName}");

                throw new UnauthorizedAccessException();
            }

            return await next();
        }
    }
}
