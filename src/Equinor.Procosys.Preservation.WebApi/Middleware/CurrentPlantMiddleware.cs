using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Equinor.Procosys.Preservation.WebApi.Middleware
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
            var headers = httpContextAccessor?.HttpContext?.Request?.Headers;
            if (headers == null)
            {
                var error = "Could not determine request headers";
                logger.LogError(error);
                context.Response.StatusCode = 400;
                context.Response.ContentType = "application/text";
                await context.Response.WriteAsync(error);
                return;
            }

            if (headers.Keys.Contains(PlantHeader))
            {
                var plant = headers[PlantHeader].ToString().ToUpperInvariant();
                plantSetter.SetPlant(plant);
            }

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}
