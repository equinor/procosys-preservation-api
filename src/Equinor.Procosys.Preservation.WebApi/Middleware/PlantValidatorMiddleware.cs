using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.MainApi.Plant;
using Equinor.Procosys.Preservation.WebApi.Misc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Equinor.Procosys.Preservation.WebApi.Middleware
{
    public class PlantValidatorMiddleware
    {
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
                    var errors = new Dictionary<string, string[]>
                    {
                        {CurrentPlantMiddleware.PlantHeader, new[] {$"Plant '{plantId}' is not a valid plant"}}
                    };
                    await context.WriteBadRequestAsync(errors, logger);
                    return;
                }
            }

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}
