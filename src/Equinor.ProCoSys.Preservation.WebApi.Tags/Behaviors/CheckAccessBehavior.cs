﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.WebApi.Tags.Authorizations;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Equinor.ProCoSys.Preservation.WebApi.Tags.Behaviors
{
    public class CheckAccessBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<CheckAccessBehavior<TRequest, TResponse>> _logger;
        private readonly IAccessValidator _accessValidator;
        public CheckAccessBehavior(ILogger<CheckAccessBehavior<TRequest, TResponse>> logger, IAccessValidator accessValidator)
        {
            _logger = logger;
            _accessValidator = accessValidator;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var typeName = request.GetGenericTypeName();

            _logger.LogInformation($"----- Checking access for {typeName}");

            if (!await _accessValidator.ValidateAsync(request as IBaseRequest))
            {
                _logger.LogWarning($"User do not have access - {typeName}");

                throw new UnauthorizedAccessException();
            }

            return await next();
        }
    }
}
