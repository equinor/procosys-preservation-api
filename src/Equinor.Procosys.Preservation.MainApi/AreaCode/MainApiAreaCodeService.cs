using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.MainApi.Client;
using Equinor.Procosys.Preservation.MainApi.Plant;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Equinor.Procosys.Preservation.MainApi.AreaCode
{
    public class MainApiAreaCodeService : IAreaCodeApiService
    {
        private readonly string _apiVersion;
        private readonly Uri _baseAddress;
        private readonly IBearerTokenApiClient _mainApiClient;
        private readonly IPlantApiService _plantApiService;
        private readonly ILogger<MainApiAreaCodeService> _logger;

        public MainApiAreaCodeService(IBearerTokenApiClient mainApiClient,
            IPlantApiService plantApiService,
            IOptionsMonitor<MainApiOptions> options,
            ILogger<MainApiAreaCodeService> logger)
        {
            _mainApiClient = mainApiClient;
            _plantApiService = plantApiService;
            _logger = logger;
            _apiVersion = options.CurrentValue.ApiVersion;
            _baseAddress = new Uri(options.CurrentValue.BaseAddress);
        }

        public async Task<List<AreaCode>> GetAreaCodes(string plant)
        {
            if (!await _plantApiService.IsPlantValidAsync(plant))
            {
                throw new ArgumentException($"Invalid plant: {plant}");
            }

            var url = $"{_baseAddress}Library/Areas" +
                $"?plantId={plant}" +
                $"&api-version={_apiVersion}";

            return await _mainApiClient.QueryAndDeserialize<List<AreaCode>>(url) ?? new List<AreaCode>();
        }
    }
}
