using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
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
        [HttpPut("Clear")]
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

        [Authorize]
        [HttpGet("Permissions")]
        public async Task<IList<string>> GetPermissions(
            [FromHeader(Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant)
        {
            var currentUserOid = _currentUserProvider.GetCurrentUserOid();
            var permissions = await _permissionCache.GetPermissionsForUserAsync(plant, currentUserOid);
            return permissions;
        }

        [Authorize]
        [HttpGet("Projects")]
        public async Task<IList<string>> GetProjects(
            [FromHeader(Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant)
        {
            var currentUserOid = _currentUserProvider.GetCurrentUserOid();
            var projects = await _permissionCache.GetProjectNamesForUserOidAsync(plant, currentUserOid);
            return projects;
        }

        [Authorize]
        [HttpGet("Plants")]
        public async Task<IList<string>> GetPlants()
        {
            var currentUserOid = _currentUserProvider.GetCurrentUserOid();
            var plants = await _plantCache.GetPlantIdsForUserOidAsync(currentUserOid);
            return plants;
        }
    }
}
