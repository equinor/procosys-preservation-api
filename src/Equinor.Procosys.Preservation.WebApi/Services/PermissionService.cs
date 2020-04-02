using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Infrastructure.Caching;
using Equinor.Procosys.Preservation.MainApi.Permission;
using Microsoft.Extensions.Options;

namespace Equinor.Procosys.Preservation.WebApi.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IPlantProvider _plantProvider;
        private readonly ICacheManager _cacheManager;
        private readonly IPermissionApiService _permissionApiService;
        private readonly IOptionsMonitor<PermissionOptions> _options;

        public PermissionService(
            IPlantProvider plantProvider, 
            ICacheManager cacheManager, 
            IPermissionApiService permissionApiService,
            IOptionsMonitor<PermissionOptions> options)
        {
            _plantProvider = plantProvider;
            _cacheManager = cacheManager;
            _permissionApiService = permissionApiService;
            _options = options;
        }

        public async Task<IList<string>> GetPermissionsForUserOidAsync(Guid userOid)
        {
            var plant = _plantProvider.Plant;
            return await _cacheManager.GetOrCreate(
                Key(userOid, plant),
                async () => await _permissionApiService.GetPermissionsAsync(plant),
                CacheDuration.Minutes,
                _options.CurrentValue.PermissionCacheMinutes);
        }

        private static string Key(Guid userOid, string plantId)
            => $"PERMISSIONS_{userOid.ToString().ToUpper()}_{plantId}";
    }
}
