using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Infrastructure.Caching;
using Equinor.Procosys.Preservation.MainApi.Permission;
using Equinor.Procosys.Preservation.MainApi.Project;
using Microsoft.Extensions.Options;

namespace Equinor.Procosys.Preservation.WebApi.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IPlantProvider _plantProvider;
        private readonly ICacheManager _cacheManager;
        private readonly IPermissionApiService _permissionApiService;
        private readonly IProjectApiService _projectApiService;
        private readonly IOptionsMonitor<PermissionOptions> _options;

        public PermissionService(
            IPlantProvider plantProvider, 
            ICacheManager cacheManager, 
            IProjectApiService projectApiService, 
            IPermissionApiService permissionApiService,
            IOptionsMonitor<PermissionOptions> options)
        {
            _plantProvider = plantProvider;
            _cacheManager = cacheManager;
            _projectApiService = projectApiService;
            _permissionApiService = permissionApiService;
            _options = options;
        }

        public async Task<IList<string>> GetPermissionsForUserOidAsync(Guid userOid)
        {
            var plant = _plantProvider.Plant;
            return await _cacheManager.GetOrCreate(
                PermissionsCacheKey(userOid, plant),
                async () => await _permissionApiService.GetPermissionsAsync(plant),
                CacheDuration.Minutes,
                _options.CurrentValue.PermissionCacheMinutes);
        }

        public async Task<IList<string>> GetProjectsForUserOidAsync(Guid userOid)
        {
            var plant = _plantProvider.Plant;
            return await _cacheManager.GetOrCreate(
                ProjectsCacheKey(userOid, plant),
                async () =>
                {
                    var projects = await _projectApiService.GetProjectsAsync(plant);
                    return projects?.Select(p => p.Name).ToList();
                },
                CacheDuration.Minutes,
                _options.CurrentValue.PermissionCacheMinutes);
        }

        public async Task<IList<string>> GetContentRestrictionsForUserOidAsync(Guid userOid)
        {
            var plant = _plantProvider.Plant;
            return await _cacheManager.GetOrCreate(
                ContentRestrictionsCacheKey(userOid, plant),
                async () => await _permissionApiService.GetContentRestrictionsAsync(plant),
                CacheDuration.Minutes,
                _options.CurrentValue.PermissionCacheMinutes);
        }

        private string ProjectsCacheKey(Guid userOid, string plant)
            => $"PROJECTS_{userOid.ToString().ToUpper()}_{plant}";

        private static string PermissionsCacheKey(Guid userOid, string plant)
            => $"PERMISSIONS_{userOid.ToString().ToUpper()}_{plant}";

        private static string ContentRestrictionsCacheKey(Guid userOid, string plant)
            => $"CONTENTRESTRICTIONS_{userOid.ToString().ToUpper()}_{plant}";
    }
}
