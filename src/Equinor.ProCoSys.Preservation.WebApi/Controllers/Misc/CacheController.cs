using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Auth.Caches;
using Equinor.ProCoSys.Auth.Permission;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Common.Misc;
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
        public async Task ClearAsync(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            CancellationToken cancellationToken)
        {
            var currentUserOid = _currentUserProvider.GetCurrentUserOid();
            await _permissionCache.ClearAllAsync(plant, currentUserOid, cancellationToken);
        }

        [Authorize]
        [HttpGet("PermissionsFromCache")]
        public async Task<IList<string>> GetPermissionsFromCacheAsync(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            CancellationToken cancellationToken)
        {
            var currentUserOid = _currentUserProvider.GetCurrentUserOid();
            var permissions = await _permissionCache.GetPermissionsForUserAsync(plant, currentUserOid, cancellationToken);
            return permissions;
        }

        [Authorize]
        [HttpGet("PermissionsFromMain")]
        public async Task<IList<string>> GetPermissionsFromMainAsync(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            CancellationToken cancellationToken)
        {
            var permissions = await _permissionApiService.GetPermissionsForCurrentUserAsync(plant, cancellationToken);
            return permissions;
        }

        [Authorize]
        [HttpGet("ProjectsFromCache")]
        public async Task<IList<AccessableProject>> GetProjectsFromCacheAsync(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            CancellationToken cancellationToken)
        {
            var currentUserOid = _currentUserProvider.GetCurrentUserOid();
            var projects = await _permissionCache.GetProjectsForUserAsync(plant, currentUserOid, cancellationToken);
            return projects;
        }

        [Authorize]
        [HttpGet("AllProjectsFromMain")]
        public async Task<IList<AccessableProject>> GetProjectsFromMainAsync(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            CancellationToken cancellationToken)
        {
            var projects = await _permissionApiService.GetAllOpenProjectsForCurrentUserAsync(plant, cancellationToken);
            return projects;
        }

        [Authorize]
        [HttpGet("PlantsFromCache")]
        public async Task<IList<string>> GetPlantsFromCacheAsync(CancellationToken cancellationToken)
        {
            var currentUserOid = _currentUserProvider.GetCurrentUserOid();
            var plants = await _permissionCache.GetPlantIdsWithAccessForUserAsync(currentUserOid, cancellationToken);
            return plants;
        }

        [Authorize]
        [HttpGet("AllPlantsFromMain")]
        public async Task<IList<AccessablePlant>> GetPlantsFromMainAsync(CancellationToken cancellationToken)
        {
            var currentUserOid = _currentUserProvider.GetCurrentUserOid();
            var plants = await _permissionApiService.GetAllPlantsForUserAsync(currentUserOid, cancellationToken);
            return plants;
        }
    }
}
