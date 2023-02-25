using System;
using System.Threading.Tasks;
using Equinor.ProCoSys.Auth.Misc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Equinor.ProCoSys.Preservation.WebApi.Middleware
{
    public class CurrentPlantMiddleware
    {
        public const string PlantHeader = "x-plant";

        private readonly RequestDelegate _next;

        public CurrentPlantMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(
            HttpContext context,
            IHttpContextAccessor httpContextAccessor,
            IPlantSetter plantSetter,
            ILogger<CurrentPlantMiddleware> logger)
        {
            logger.LogInformation($"----- {GetType().Name} start");
            var headers = httpContextAccessor?.HttpContext?.Request?.Headers;
            if (headers == null)
            {
                throw new Exception("Could not determine request headers");
            }

            if (headers.Keys.Contains(PlantHeader))
            {
                var plant = headers[PlantHeader].ToString().ToUpperInvariant();
                plantSetter.SetPlant(plant);
            }

            logger.LogInformation($"----- {GetType().Name} complete");
            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}
