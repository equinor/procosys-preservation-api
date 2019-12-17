using System;
using Equinor.Procosys.Preservation.Domain;
using Microsoft.AspNetCore.Http;

namespace Equinor.Procosys.Preservation.WebApi.Middleware
{
    public class PlantProvider : IPlantProvider
    {
        private readonly IHttpContextAccessor _accessor;

        public PlantProvider(IHttpContextAccessor accessor) => _accessor = accessor;

        public string Plant => _accessor?.HttpContext?.Request?.Headers["x-plant"] ?? throw new Exception("Could not determine current plant");
    }
}
