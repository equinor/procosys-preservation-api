using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.MainApi.Plant;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Equinor.Procosys.Preservation.WebApi.Middleware
{
    public class PlantValidatorMiddleware
    {
        public const string PlantHeader = "x-plant";

        private readonly RequestDelegate _next;

        public PlantValidatorMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(
            HttpContext context,
            IPlantProvider plantProvider,
            IPlantCache plantCache,
            ILogger<PlantValidatorMiddleware> logger)
        {
            var plantId = plantProvider.Plant;
            if (context.User.Identity.IsAuthenticated && plantId != null)
            {
                if (!await plantCache.IsAValidPlantAsync(plantId))
                {
                    var error = $"Plant '{plantId}' is not a valid plant";
                    logger.LogError(error);
                    context.Response.StatusCode = 400;
                    context.Response.ContentType = "application/text";
                    await context.Response.WriteAsync(error);
                    return;
                }
            }

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}
