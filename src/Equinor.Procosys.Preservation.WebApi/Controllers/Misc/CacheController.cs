using System.ComponentModel.DataAnnotations;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.MainApi.Plant;
using Equinor.Procosys.Preservation.WebApi.Caches;
using Equinor.Procosys.Preservation.WebApi.Misc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Misc
{
    [ApiController]
    [Route("Cache")]
    public class CacheController : ControllerBase
    {
        private readonly IPlantCache _plantCache;
        private readonly IPermissionCache _permissionCache;
        private readonly ICurrentUserProvider _currentUserProvider;

        public CacheController(IPlantCache plantCache, IPermissionCache permissionCache, ICurrentUserProvider currentUserProvider)
        {
            _plantCache = plantCache;
            _permissionCache = permissionCache;
            _currentUserProvider = currentUserProvider;
        }

        [Authorize]
        [HttpGet("Clear")]
        public void Clear(
            [FromHeader(Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant)
        {
            var currentUserOid = _currentUserProvider.GetCurrentUserOid();
            _plantCache.Clear(currentUserOid);
            _permissionCache.ClearAll(plant, currentUserOid);
        }
    }
}
