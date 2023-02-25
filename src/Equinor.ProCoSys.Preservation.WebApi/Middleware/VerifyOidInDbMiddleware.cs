using System;
using System.Threading.Tasks;
using Equinor.ProCoSys.Auth.Authorization;
using Equinor.ProCoSys.Preservation.Command.PersonCommands.CreatePerson;
using Equinor.ProCoSys.Preservation.WebApi.Authorizations;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Equinor.ProCoSys.Preservation.WebApi.Middleware
{
    public class VerifyOidInDbMiddleware
    {
        private readonly RequestDelegate _next;

        public VerifyOidInDbMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(
            HttpContext context,
            IHttpContextAccessor httpContextAccessor,
            IMediator mediator,
            ILogger<VerifyOidInDbMiddleware> logger)
        {
            logger.LogInformation($"----- {GetType().Name} start");
            var httpContextUser = httpContextAccessor?.HttpContext?.User;
            var oid = httpContextUser?.Claims.TryGetOid();
            if (oid.HasValue)
            {
                var command = new CreatePersonCommand(oid.Value);
                try
                {
                    await mediator.Send(command);
                }
                catch (Exception e)
                {
                    // We have to do this silently as concurrency is a very likely problem.
                    // For a user accessing preservation for the first time, there will probably be multiple
                    // requests in parallel.
                    logger.LogError($"Exception handling {nameof(CreatePersonCommand)}", e);
                    throw;
                }
            }
            
            logger.LogInformation($"----- {GetType().Name} complete");
            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}
