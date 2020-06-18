using System;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.WebApi.Misc;
using Microsoft.AspNetCore.Http;

namespace Equinor.Procosys.Preservation.WebApi.Middleware
{
    public class CurrentPlantMiddleware
    {
        private readonly RequestDelegate _next;

        public CurrentPlantMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context, IHttpContextAccessor httpContextAccessor, IPlantSetter plantSetter)
        {
            var plant = httpContextAccessor?.HttpContext?.Request?.Headers[PlantProvider.PlantHeader].ToString().ToUpperInvariant() ??
                        throw new Exception("Could not determine current plant");
            plantSetter.SetPlant(plant);

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}
