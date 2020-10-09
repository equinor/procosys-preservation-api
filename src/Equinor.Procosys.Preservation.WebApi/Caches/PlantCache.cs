using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Infrastructure.Caching;
using Equinor.Procosys.Preservation.MainApi.Plant;
using Microsoft.Extensions.Options;

namespace Equinor.Procosys.Preservation.WebApi.Caches
{
    public class PlantCache : IPlantCache
    {
        private readonly ICacheManager _cacheManager;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IPlantApiService _plantApiService;
        private readonly IOptionsMonitor<CacheOptions> _options;

        public PlantCache(
            ICacheManager cacheManager, 
            ICurrentUserProvider currentUserProvider, 
            IPlantApiService plantApiService, 
            IOptionsMonitor<CacheOptions> options)
        {
            _cacheManager = cacheManager;
            _currentUserProvider = currentUserProvider;
            _plantApiService = plantApiService;
            _options = options;
        }

        public async Task<IList<string>> GetPlantWithAccessForUserAsync(Guid userOid)
        {
            var allPlants = await GetAllPlantsForUserAsync(userOid);
            return allPlants?.Where(p => p.HasAccess).Select(p => p.Id).ToList();
        }

        public async Task<bool> HasUserAccessToPlantAsync(string plantId, Guid userOid)
        {
            var plantIds = await GetPlantWithAccessForUserAsync(userOid);
            return plantIds.Contains(plantId);
        }

        public async Task<bool> HasCurrentUserAccessToPlantAsync(string plantId)
        {
            var userOid = _currentUserProvider.GetCurrentUserOid();

            return await HasUserAccessToPlantAsync(plantId, userOid);
        }

        public async Task<bool> IsAValidPlantAsync(string plantId)
        {
            var userOid = _currentUserProvider.GetCurrentUserOid();
            var allPlants = await GetAllPlantsForUserAsync(userOid);
            return allPlants != null && allPlants.Any(p => p.Id == plantId);
        }

        public void Clear(Guid userOid) => _cacheManager.Remove(PlantsCacheKey(userOid));

        private async Task<IList<ProcosysPlant>> GetAllPlantsForUserAsync(Guid userOid)
            => await _cacheManager.GetOrCreate(
                PlantsCacheKey(userOid),
                async () =>
                {
                    var plants = await _plantApiService.GetAllPlantsAsync();
                    return plants;
                },
                CacheDuration.Minutes,
                _options.CurrentValue.PlantCacheMinutes);

        private string PlantsCacheKey(Guid userOid)
            => $"PLANTS_{userOid.ToString().ToUpper()}";
    }
}
