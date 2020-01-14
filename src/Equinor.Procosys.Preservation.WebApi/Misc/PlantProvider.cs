using System;
using Equinor.Procosys.Preservation.Domain;
using Microsoft.AspNetCore.Http;

namespace Equinor.Procosys.Preservation.WebApi.Misc
{
    public class PlantProvider : IPlantProvider
    {
        public static string PlantHeader = "x-plant";

        private readonly IHttpContextAccessor _accessor;

        public PlantProvider(IHttpContextAccessor accessor) => _accessor = accessor;

        public string Plant => _accessor?.HttpContext?.Request?.Headers[PlantHeader].ToString().ToUpperInvariant() ?? throw new Exception("Could not determine current plant");
    }
}
