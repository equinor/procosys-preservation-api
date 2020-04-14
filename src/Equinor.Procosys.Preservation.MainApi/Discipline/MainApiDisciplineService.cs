using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.MainApi.Client;
using Equinor.Procosys.Preservation.MainApi.Plant;
using Microsoft.Extensions.Options;


namespace Equinor.Procosys.Preservation.MainApi.Discipline
{
    public class MainApiDisciplineService : IDisciplineApiService
    {
        private readonly string _apiVersion;
        private readonly Uri _baseAddress;
        private readonly IBearerTokenApiClient _mainApiClient;
        private readonly IPlantApiService _plantApiService;

        public MainApiDisciplineService(IBearerTokenApiClient mainApiClient,
            IPlantApiService plantApiService,
            IOptionsMonitor<MainApiOptions> options)
        {
            _mainApiClient = mainApiClient;
            _plantApiService = plantApiService;
            _apiVersion = options.CurrentValue.ApiVersion;
            _baseAddress = new Uri(options.CurrentValue.BaseAddress);
        }

        public async Task<List<ProcosysDiscipline>> GetDisciplinesAsync(string plant)
        {
            if (!await _plantApiService.IsPlantValidAsync(plant))
            {
                throw new ArgumentException($"Invalid plant: {plant}");
            }

            var url = $"{_baseAddress}Library/Disciplines" +
                      $"?plantId={plant}" +
                      "&classifications=PRESERVATION" +
                      $"&api-version={_apiVersion}";

            return await _mainApiClient.QueryAndDeserialize<List<ProcosysDiscipline>>(url) ?? new List<ProcosysDiscipline>();
        }

        public async Task<ProcosysDiscipline> GetDisciplineAsync(string plant, string code)
        {
            if (!await _plantApiService.IsPlantValidAsync(plant))
            {
                throw new ArgumentException($"Invalid plant: {plant}");
            }

            var url = $"{_baseAddress}Library/Discipline" +
                      $"?plantId={plant}" +
                      $"&code={code}" +
                      $"&api-version={_apiVersion}";

            return await _mainApiClient.QueryAndDeserialize<ProcosysDiscipline>(url);
        }
    }
}
