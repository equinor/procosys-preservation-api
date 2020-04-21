using System;
using Microsoft.AspNetCore.Http;

namespace Equinor.Procosys.Preservation.WebApi.Authorizations
{
    public class PlantAccessChecker : IPlantAccessChecker
    {
        private readonly IHttpContextAccessor _accessor;

        public PlantAccessChecker(IHttpContextAccessor accessor) => _accessor = accessor;

        public bool HasCurrentUserAccessToPlant(string plantId)
        {
            if (string.IsNullOrEmpty(plantId))
            {
                throw new ArgumentNullException(nameof(plantId));
            }

            var user = _accessor.HttpContext.User;
            
            return !user.Identity.IsAuthenticated || user.Claims.HasPlantClaim(plantId);
        }
    }
}
