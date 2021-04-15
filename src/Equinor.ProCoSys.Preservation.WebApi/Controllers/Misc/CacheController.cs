using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.MainApi.Permission;
using Equinor.ProCoSys.Preservation.MainApi.Plant;
using Equinor.ProCoSys.Preservation.WebApi.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Equinor.ProCoSys.Preservation.WebApi.Controllers.Misc
{
    [ApiController]
    [Route("Cache")]
    public class CacheController : ControllerBase
    {
        private readonly IPlantCache _plantCache;
        private readonly IPermissionCache _permissionCache;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IPermissionApiService _permissionApiService;
        private readonly IPlantApiService _plantApiService;

        public CacheController(
            IPlantCache plantCache,
            IPermissionCache permissionCache,
            ICurrentUserProvider currentUserProvider,
            IPermissionApiService permissionApiService,
            IPlantApiService plantApiService)
        {
            _plantCache = plantCache;
            _permissionCache = permissionCache;
            _currentUserProvider = currentUserProvider;
            _permissionApiService = permissionApiService;
            _plantApiService = plantApiService;
        }

        [Authorize]
        [HttpPut("Clear")]
        public void Clear(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant)
        {
            var currentUserOid = _currentUserProvider.GetCurrentUserOid();
            _plantCache.Clear(currentUserOid);
            _permissionCache.ClearAll(plant, currentUserOid);
        }

        [Authorize]
        [HttpGet("PermissionsFromCache")]
        public async Task<IList<string>> GetPermissionsFromCache(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant)
        {
            var currentUserOid = _currentUserProvider.GetCurrentUserOid();
            var permissions = await _permissionCache.GetPermissionsForUserAsync(plant, currentUserOid);
            return permissions;
        }

        [Authorize]
        [HttpGet("PermissionsFromMain")]
        public async Task<IList<string>> GetPermissionsFromMain(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant)
        {
            var permissions = await _permissionApiService.GetPermissionsAsync(plant);
            return permissions;
        }

        [Authorize]
        [HttpGet("ProjectsFromCache")]
        public async Task<IList<string>> GetProjectsFromCache(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant)
        {
            var currentUserOid = _currentUserProvider.GetCurrentUserOid();
            var projects = await _permissionCache.GetProjectsForUserAsync(plant, currentUserOid);
            return projects;
        }

        [Authorize]
        [HttpGet("AllProjectsFromMain")]
        public async Task<IList<PCSProject>> GetProjectsFromMain(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant)
        {
            var projects = await _permissionApiService.GetAllOpenProjectsAsync(plant);
            return projects;
        }

        [Authorize]
        [HttpGet("PlantsFromCache")]
        public async Task<IList<string>> GetPlantsFromCache()
        {
            var currentUserOid = _currentUserProvider.GetCurrentUserOid();
            var plants = await _plantCache.GetPlantWithAccessForUserAsync(currentUserOid);
            return plants;
        }

        [Authorize]
        [HttpGet("AllPlantsFromMain")]
        public async Task<IList<PCSPlant>> GetPlantsFromMain()
        {
            var plants = await _plantApiService.GetAllPlantsAsync();
            return plants;
        }
    }
}
