using System.Threading.Tasks;
using Equinor.ProCoSys.Auth.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Equinor.ProCoSys.Preservation.WebApi.Middleware
{
    public class CurrentBearerTokenMiddleware
    {
        private readonly RequestDelegate _next;

        public CurrentBearerTokenMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(
            HttpContext context,
            IHttpContextAccessor httpContextAccessor,
            IBearerTokenSetterForAll bearerTokenSetterForAll,
            ILogger<CurrentBearerTokenMiddleware> logger)
        {
            logger.LogInformation($"----- {GetType().Name} start");
            var authorizationHeader = httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            var tokens = authorizationHeader.ToString()?.Split(' ');

            if (tokens != null && tokens.Length > 1)
            {
                var token = tokens[1];
                bearerTokenSetterForAll.SetBearerToken(token);
            }

            logger.LogInformation($"----- {GetType().Name} complete");
            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}
