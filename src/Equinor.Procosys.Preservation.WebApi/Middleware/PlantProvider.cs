using Equinor.Procosys.Preservation.Infrastructure;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;

namespace Equinor.Procosys.Preservation.WebApi.Middleware
{
    public class PlantProvider : IPlantProvider
    {
        private readonly IHttpContextAccessor accessor;

        public PlantProvider(IHttpContextAccessor accessor)
        {
            this.accessor = accessor;
        }

        public string Plant => accessor?.HttpContext?.User?.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Name)?.Value ?? throw new ArgumentException("Could not determine current plant");
    }
}
