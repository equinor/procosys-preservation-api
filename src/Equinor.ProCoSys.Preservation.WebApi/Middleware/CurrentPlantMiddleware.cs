using System;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
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
            logger.LogInformation("----- {Name} start", GetType().Name);
            var headers = httpContextAccessor?.HttpContext?.Request.Headers;
            if (headers == null)
            {
                throw new Exception("Could not determine request headers");
            }

            if (headers.Keys.Select(h=> h.ToString()).Contains(PlantHeader, StringComparer.OrdinalIgnoreCase))
            {
                var plant = headers[PlantHeader].ToString().ToUpperInvariant();
                plantSetter.SetPlant(plant);
                logger.LogInformation("----- {Name} complete setting plant {Plant}", GetType().Name, plant);
            }
            else
            {
                logger.LogInformation("----- {Name} complete. No plant set", GetType().Name);
            }
            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}
