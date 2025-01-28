using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Auth.Authorization;
using Equinor.ProCoSys.Auth.Caches;
using Equinor.ProCoSys.Common.Misc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Equinor.ProCoSys.Preservation.WebApi.Middleware
{
    public class PersonValidatorMiddleware
    {
        private readonly RequestDelegate _next;

        public PersonValidatorMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(
            HttpContext context,
            ICurrentUserProvider currentUserProvider,
            ILocalPersonRepository localPersonRepository,
            IPersonCache personCache,
            ILogger<PersonValidatorMiddleware> logger)
        {
            logger.LogInformation($"----- {GetType().Name} start");
            if (currentUserProvider.HasCurrentUser)
            {
                var oid = currentUserProvider.GetCurrentUserOid();
                if (!await localPersonRepository.ExistsAsync(oid, CancellationToken.None) &&
                    !await personCache.ExistsAsync(oid, CancellationToken.None))
                {
                    await context.WriteForbidden(logger);
                    return;
                }
            }

            logger.LogInformation($"----- {GetType().Name} complete");
            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}
