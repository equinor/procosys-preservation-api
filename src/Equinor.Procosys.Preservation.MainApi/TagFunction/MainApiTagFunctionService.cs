using System;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.MainApi.Client;
using Equinor.Procosys.Preservation.MainApi.Plant;
using Microsoft.Extensions.Options;

namespace Equinor.Procosys.Preservation.MainApi.TagFunction
{
    public class MainApiTagFunctionService : ITagFunctionApiService
    {
        private readonly string _apiVersion;
        private readonly Uri _baseAddress;
        private readonly IBearerTokenApiClient _mainApiClient;
        private readonly IPlantApiService _plantApiService;

        public MainApiTagFunctionService(IBearerTokenApiClient mainApiClient,
            IPlantApiService plantApiService,
            IOptionsMonitor<MainApiOptions> options)
        {
            _mainApiClient = mainApiClient;
            _plantApiService = plantApiService;
            _apiVersion = options.CurrentValue.ApiVersion;
            _baseAddress = new Uri(options.CurrentValue.BaseAddress);
        }

        public async Task<ProcosysTagFunction> GetTagFunctionAsync(string plant, string tagFunctionCode, string registerCode)
        {
            if (!await _plantApiService.IsPlantValidAsync(plant))
            {
                throw new ArgumentException($"Invalid plant: {plant}");
            }

            var url = $"{_baseAddress}Library/TagFunction" +
                $"?plantId={plant}" +
                $"&tagFunctionCode={tagFunctionCode}" +
                $"&registerCode={registerCode}" +
                $"&api-version={_apiVersion}";

            return await _mainApiClient.QueryAndDeserialize<ProcosysTagFunction>(url);
        }
    }
}
