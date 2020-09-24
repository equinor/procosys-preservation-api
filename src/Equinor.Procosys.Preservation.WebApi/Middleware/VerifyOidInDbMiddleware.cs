using System;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.PersonCommands.CreatePerson;
using Equinor.Procosys.Preservation.WebApi.Authorizations;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Equinor.Procosys.Preservation.WebApi.Middleware
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
            var httpContextUser = httpContextAccessor.HttpContext.User;
            var oid = httpContextUser.Claims.TryGetOid();
            if (oid.HasValue)
            {
                var givenName = httpContextUser.Claims.TryGetGivenName();
                var surName = httpContextUser.Claims.TryGetSurName();

                if (string.IsNullOrEmpty(givenName) && string.IsNullOrEmpty(surName))
                {
                    var fullName = httpContextUser.Claims.TryGetFullName();
                    // Last name will be set to the last part of the name, regardless of any middle-name variants (best effort).
                    var indexOfLastSpace = fullName.LastIndexOf(" ", StringComparison.InvariantCulture);
                    givenName = fullName.Substring(0, indexOfLastSpace);
                    surName = fullName.Substring(indexOfLastSpace + 1);
                }

                var command = new CreatePersonCommand(oid.Value, givenName, surName);
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
                }
            }
            
            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}
