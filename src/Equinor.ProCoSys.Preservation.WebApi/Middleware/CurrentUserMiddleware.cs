using System.Threading.Tasks;
using Equinor.ProCoSys.Auth.Authorization;
using Equinor.ProCoSys.Common.Misc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Equinor.ProCoSys.Preservation.WebApi.Middleware
{
    public class CurrentUserMiddleware
    {
        private readonly RequestDelegate _next;

        public CurrentUserMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(
            HttpContext context,
            IHttpContextAccessor httpContextAccessor,
            ICurrentUserSetter currentUserSetter,
            ILogger<CurrentUserMiddleware> logger)
        {
            logger.LogInformation($"----- {GetType().Name} start");
            var oid = httpContextAccessor.HttpContext?.User.Claims.TryGetOid();
            if (oid.HasValue)
            {
                currentUserSetter.SetCurrentUserOid(oid.Value);
            }

            logger.LogInformation($"----- {GetType().Name} complete");
            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}
