using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.WebApi.Misc;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Equinor.Procosys.Preservation.WebApi.Behaviors
{
    public class CheckValidProjectBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
    {
        private readonly ILogger<CheckValidProjectBehavior<TRequest, TResponse>> _logger;
        private readonly IProjectChecker _projectChecker;

        public CheckValidProjectBehavior(ILogger<CheckValidProjectBehavior<TRequest, TResponse>> logger, IProjectChecker projectChecker)
        {
            _logger = logger;
            _projectChecker = projectChecker;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var typeName = request.GetGenericTypeName();

            _logger.LogInformation($"----- Checking project for {typeName}");

            await _projectChecker.EnsureValidProjectAsync(request as IBaseRequest);

            return await next();
        }
    }
}
