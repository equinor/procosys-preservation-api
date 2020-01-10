using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.MainApi.Client;

namespace Equinor.Procosys.Preservation.MainApi.Plant
{
    public class MainApiPlantService : IPlantApiService
    {
        private const string ApiVersion = "4.0";
        private readonly IMainApiClient _mainApiClient;

        public MainApiPlantService(
            IMainApiClient mainApiClient)
        {
            _mainApiClient = mainApiClient;
        }

        public Task<List<ProcosysPlant>> GetPlants() => _mainApiClient.QueryAndDeserialize<List<ProcosysPlant>>($"Plants?api-version={ApiVersion}");

        public async Task<bool> IsPlantValidAsync(string plant)
        {
            var plants = await GetPlants();
            return plants.Any(p => p.Id == plant);
        }
    }
}
