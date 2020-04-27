using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Infrastructure.Caching;
using Equinor.Procosys.Preservation.MainApi.Permission;
using Microsoft.Extensions.Options;

namespace Equinor.Procosys.Preservation.WebApi.Caches
{
    public class PermissionCache : IPermissionCache
    {
        private readonly ICacheManager _cacheManager;
        private readonly IPermissionApiService _permissionApiService;
        private readonly IOptionsMonitor<CacheOptions> _options;

        public PermissionCache(
            ICacheManager cacheManager,
            IPermissionApiService permissionApiService,
            IOptionsMonitor<CacheOptions> options)
        {
            _cacheManager = cacheManager;
            _permissionApiService = permissionApiService;
            _options = options;
        }

        public async Task<IList<string>> GetPermissionsForUserAsync(string plantId, Guid userOid)
            => await _cacheManager.GetOrCreate(
                PermissionsCacheKey(userOid, plantId),
                async () => await _permissionApiService.GetPermissionsAsync(plantId),
                CacheDuration.Minutes,
                _options.CurrentValue.PermissionCacheMinutes);

        public async Task<IList<string>> GetProjectNamesForUserOidAsync(string plantId, Guid userOid)
            => await _cacheManager.GetOrCreate(
                ProjectsCacheKey(userOid, plantId),
                async () => await _permissionApiService.GetProjectsAsync(plantId),
                CacheDuration.Minutes,
                _options.CurrentValue.PermissionCacheMinutes);

        public async Task<IList<string>> GetContentRestrictionsForUserOidAsync(string plantId, Guid userOid)
            => await _cacheManager.GetOrCreate(
                ContentRestrictionsCacheKey(userOid, plantId),
                async () => await _permissionApiService.GetContentRestrictionsAsync(plantId),
                CacheDuration.Minutes,
                _options.CurrentValue.PermissionCacheMinutes);

        private string ProjectsCacheKey(Guid userOid, string plant)
            => $"PROJECTS_{userOid.ToString().ToUpper()}_{plant}";

        private static string PermissionsCacheKey(Guid userOid, string plant)
            => $"PERMISSIONS_{userOid.ToString().ToUpper()}_{plant}";

        private static string ContentRestrictionsCacheKey(Guid userOid, string plant)
            => $"CONTENTRESTRICTIONS_{userOid.ToString().ToUpper()}_{plant}";
    }
}
