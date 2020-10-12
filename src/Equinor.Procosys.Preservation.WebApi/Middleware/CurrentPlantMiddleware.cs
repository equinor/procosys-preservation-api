using System;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Microsoft.AspNetCore.Http;

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
            IPlantSetter plantSetter)
        {
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

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}
