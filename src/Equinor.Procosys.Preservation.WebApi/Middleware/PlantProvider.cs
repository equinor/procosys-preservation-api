using System;
using Equinor.Procosys.Preservation.Domain;
using Microsoft.AspNetCore.Http;

namespace Equinor.Procosys.Preservation.WebApi.Middleware
{
    public class PlantProvider : IPlantProvider
    {
        private readonly IHttpContextAccessor accessor;

        public PlantProvider(IHttpContextAccessor accessor)
        {
            this.accessor = accessor;
        }

        public string Plant => accessor?.HttpContext?.Request?.Headers["x-plant"] ?? throw new ArgumentException("Could not determine current plant");
    }
}
