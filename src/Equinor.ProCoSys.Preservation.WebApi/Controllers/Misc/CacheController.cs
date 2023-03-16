using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Equinor.ProCoSys.Auth.Caches;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Auth.Permission;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.WebApi.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Equinor.ProCoSys.Preservation.WebApi.Controllers.Misc
{
    [ApiController]
    [Route("Cache")]
    public class CacheController : ControllerBase
    {
        private readonly IPermissionCache _permissionCache;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IPermissionApiService _permissionApiService;

        public CacheController(
            IPermissionCache permissionCache,
            ICurrentUserProvider currentUserProvider,
            IPermissionApiService permissionApiService)
        {
            _permissionCache = permissionCache;
            _permissionCache = permissionCache;
            _currentUserProvider = currentUserProvider;
            _permissionApiService = permissionApiService;
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
            var permissions = await _permissionApiService.GetPermissionsForCurrentUserAsync(plant);
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
        public async Task<IList<AccessableProject>> GetProjectsFromMain(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant)
        {
            var projects = await _permissionApiService.GetAllOpenProjectsForCurrentUserAsync(plant);
            return projects;
        }

        [Authorize]
        [HttpGet("PlantsFromCache")]
        public async Task<IList<string>> GetPlantsFromCache()
        {
            var currentUserOid = _currentUserProvider.GetCurrentUserOid();
            var plants = await _permissionCache.GetPlantIdsWithAccessForUserAsync(currentUserOid);
            return plants;
        }

        [Authorize]
        [HttpGet("AllPlantsFromMain")]
        public async Task<IList<AccessablePlant>> GetPlantsFromMain()
        {
            var currentUserOid = _currentUserProvider.GetCurrentUserOid();
            var plants = await _permissionApiService.GetAllPlantsForUserAsync(currentUserOid);
            return plants;
        }
    }
}
