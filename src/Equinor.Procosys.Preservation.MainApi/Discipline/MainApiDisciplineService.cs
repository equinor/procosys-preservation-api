using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.MainApi.Client;
using Equinor.Procosys.Preservation.MainApi.Plant;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace Equinor.Procosys.Preservation.MainApi.Discipline
{
    public class MainApiDisciplineService : IDisciplineApiService
    {
        private readonly string _apiVersion;
        private readonly Uri _baseAddress;
        private readonly IBearerTokenApiClient _mainApiClient;
        private readonly IPlantApiService _plantApiService;
        private readonly ILogger<MainApiDisciplineService> _logger;

        public MainApiDisciplineService(IBearerTokenApiClient mainApiClient,
            IPlantApiService plantApiService,
            IOptionsMonitor<MainApiOptions> options,
            ILogger<MainApiDisciplineService> logger)
        {
            _mainApiClient = mainApiClient;
            _plantApiService = plantApiService;
            _logger = logger;
            _apiVersion = options.CurrentValue.ApiVersion;
            _baseAddress = new Uri(options.CurrentValue.BaseAddress);
        }

        public async Task<List<ProcosysDiscipline>> GetDisciplines(string plant)
        {
            if (!await _plantApiService.IsPlantValidAsync(plant))
            {
                throw new ArgumentException($"Invalid plant: {plant}");
            }

            var url = $"{_baseAddress}Library/Disciplines" +
                $"?plantId={plant}" +
                $"&api-version={_apiVersion}";

            return await _mainApiClient.QueryAndDeserialize<List<ProcosysDiscipline>>(url) ?? new List<ProcosysDiscipline>();
        }
    }
}
