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
        private readonly IMainApiClient _mainApiClient;

        public MainApiPlantService(
            IMainApiClient mainApiClient,
            IOptionsMonitor<MainApiOptions> options)
        {
            _mainApiClient = mainApiClient;
            _apiVersion = options.CurrentValue.ApiVersion;
        }

        public Task<List<ProcosysPlant>> GetPlants() => _mainApiClient.QueryAndDeserialize<List<ProcosysPlant>>($"Plants?api-version={_apiVersion}");

        public async Task<bool> IsPlantValidAsync(string plant)
        {
            var plants = await GetPlants();
            return plants.Any(p => p.Id == plant);
        }
    }
}
