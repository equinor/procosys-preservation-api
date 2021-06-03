using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Infrastructure.Caching;
using Equinor.ProCoSys.Preservation.MainApi.Plant;
using Microsoft.Extensions.Options;

namespace Equinor.ProCoSys.Preservation.WebApi.Caches
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

        public async Task<IList<string>> GetPlantIdsWithAccessForUserAsync(Guid userOid)
        {
            var allPlants = await GetAllPlantsForUserAsync(userOid);
            return allPlants?.Where(p => p.HasAccess).Select(p => p.Id).ToList();
        }

        public async Task<bool> HasUserAccessToPlantAsync(string plantId, Guid userOid)
        {
            var plantIds = await GetPlantIdsWithAccessForUserAsync(userOid);
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

        public async Task<string> GetPlantTitleAsync(string plantId)
        {
            var userOid = _currentUserProvider.GetCurrentUserOid();
            var allPlants = await GetAllPlantsForUserAsync(userOid);
            return allPlants?.Where(p => p.Id == plantId).SingleOrDefault()?.Title;
        }

        public void Clear(Guid userOid) => _cacheManager.Remove(PlantsCacheKey(userOid));

        private async Task<IList<PCSPlant>> GetAllPlantsForUserAsync(Guid userOid)
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
