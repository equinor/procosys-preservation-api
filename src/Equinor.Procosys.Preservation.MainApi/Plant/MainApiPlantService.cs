using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.MainApi.Client;
using Microsoft.Extensions.Options;

namespace Equinor.Procosys.Preservation.MainApi.Plant
{
    public class MainApiPlantService : IPlantApiService
    {
        private readonly string _apiVersion;
        private readonly Uri _baseAddress;
        private readonly IBearerTokenApiClient _mainApiClient;

        public MainApiPlantService(
            IBearerTokenApiClient mainApiClient,
            IOptionsMonitor<MainApiOptions> options)
        {
            _mainApiClient = mainApiClient;
            _apiVersion = options.CurrentValue.ApiVersion;
            _baseAddress = new Uri(options.CurrentValue.BaseAddress);
        }

        public async Task<List<ProcosysPlant>> GetPlantsAsync()
        {
            var url = $"{_baseAddress}Plants?api-version={_apiVersion}";
            return await _mainApiClient.QueryAndDeserialize<List<ProcosysPlant>>(url);
        }

        public async Task<bool> IsPlantValidAsync(string plant)
        {
            var plants = await GetPlantsAsync();
            return plants.Any(p => p.Id == plant);
        }
    }
}
