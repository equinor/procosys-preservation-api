using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Infrastructure.Caching;
using Equinor.ProCoSys.Preservation.MainApi.Me;
using Equinor.ProCoSys.Preservation.MainApi.Permission;
using Microsoft.Extensions.Options;

namespace Equinor.ProCoSys.Preservation.WebApi.Caches
{
    public class PermissionCache : IPermissionCache
    {
        private readonly ICacheManager _cacheManager;
        private readonly IPermissionApiService _permissionApiService;
        private readonly IMeApiService _meApiService;
        private readonly IOptionsSnapshot<CacheOptions> _options;

        public PermissionCache(
            ICacheManager cacheManager,
            IPermissionApiService permissionApiService,
            IMeApiService meApiService,
            IOptionsSnapshot<CacheOptions> options)
        {
            _cacheManager = cacheManager;
            _permissionApiService = permissionApiService;
            _meApiService = meApiService;
            _options = options;
        }

        public async Task<IList<string>> GetPermissionsForUserAsync(string plantId, Guid userOid)
            => await _cacheManager.GetOrCreate(
                PermissionsCacheKey(plantId, userOid),
                async () => await _permissionApiService.GetPermissionsAsync(plantId),
                CacheDuration.Minutes,
                _options.Value.PermissionCacheMinutes);

        public async Task<IList<string>> GetProjectsForUserAsync(string plantId, Guid userOid)
        {
            var allProjects = await GetAllProjectsForUserAsync(plantId, userOid);
            return allProjects?.Where(p => p.HasAccess).Select(p => p.Name).ToList();
        }

        public async Task<bool> IsAValidProjectAsync(string plantId, Guid userOid, string projectName)
        {
            var allProjects = await GetAllProjectsForUserAsync(plantId, userOid);
            return allProjects != null && allProjects.Any(p => p.Name == projectName);
        }

        public async Task<IList<string>> GetContentRestrictionsForUserAsync(string plantId, Guid userOid)
            => await _cacheManager.GetOrCreate(
                ContentRestrictionsCacheKey(plantId, userOid),
                async () => await _permissionApiService.GetContentRestrictionsAsync(plantId),
                CacheDuration.Minutes,
                _options.Value.PermissionCacheMinutes);

        public void ClearAll(string plantId, Guid userOid)
        {
            _cacheManager.Remove(ProjectsCacheKey(plantId, userOid));
            _cacheManager.Remove(PermissionsCacheKey(plantId, userOid));
            _cacheManager.Remove(ContentRestrictionsCacheKey(plantId, userOid));
        }

        private async Task<IList<PCSProject>> GetAllProjectsForUserAsync(string plantId, Guid userOid)
            => await _cacheManager.GetOrCreate(
                ProjectsCacheKey(plantId, userOid),
                async () => await GetAllOpenProjectsAsync(plantId),
                CacheDuration.Minutes,
                _options.Value.PermissionCacheMinutes);

        private string ProjectsCacheKey(string plantId, Guid userOid)
        {
            if (userOid == Guid.Empty)
            {
                throw new Exception("Illegal userOid for cache");
            }
            return $"PROJECTS_{userOid.ToString().ToUpper()}_{plantId}";
        }

        private static string PermissionsCacheKey(string plantId, Guid userOid)
        {
            if (userOid == Guid.Empty)
            {
                throw new Exception("Illegal userOid for cache");
            }
            return $"PERMISSIONS_{userOid.ToString().ToUpper()}_{plantId}";
        }

        private static string ContentRestrictionsCacheKey(string plantId, Guid userOid)
        {
            if (userOid == Guid.Empty)
            {
                throw new Exception("Illegal userOid for cache");
            }
            return $"CONTENTRESTRICTIONS_{userOid.ToString().ToUpper()}_{plantId}";
        }

        private async Task<IList<PCSProject>> GetAllOpenProjectsAsync(string plantId)
        {
            // trace users use of plant each time getting projects
            // this will serve the purpose since we want to log once a day pr user pr plant, and preservation client ALWAYS get projects at startup
            await _meApiService.TracePlantAsync(plantId);
            return await _permissionApiService.GetAllOpenProjectsAsync(plantId);
        }
    }
}
