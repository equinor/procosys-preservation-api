using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.WebApi.Misc;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Equinor.Procosys.Preservation.WebApi.Behaviors
{
    public class ValidatorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<ValidatorBehavior<TRequest, TResponse>> _logger;
        private readonly IValidator<TRequest> _validator;

        public ValidatorBehavior(IValidator<TRequest> validator, ILogger<ValidatorBehavior<TRequest, TResponse>> logger)
        {
            _validator = validator;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var typeName = request.GetGenericTypeName();

            _logger.LogInformation("----- Validating command {CommandType}", typeName);

            var failures = (await _validator
                .ValidateAsync(request))
                .Errors
                .Where(error => error != null)
                .ToList();

            if (failures.Any())
            {
                _logger.LogWarning("Validation errors - {CommandType} - Command: {@Command} - Errors: {@ValidationErrors}", typeName, request, failures);

                throw new ValidationException("Validation errors", failures);
            }

            return await next();
        }
    }
}
