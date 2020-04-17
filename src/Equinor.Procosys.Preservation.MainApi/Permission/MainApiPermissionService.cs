using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.MainApi.Client;
using Equinor.Procosys.Preservation.MainApi.Plant;
using Microsoft.Extensions.Options;

namespace Equinor.Procosys.Preservation.MainApi.Permission
{
    public class MainApiPermissionService : IPermissionApiService
    {
        private readonly string _apiVersion;
        private readonly Uri _baseAddress;
        private readonly IBearerTokenApiClient _mainApiClient;
        private readonly IPlantApiService _plantApiService;

        public MainApiPermissionService(IBearerTokenApiClient mainApiClient,
            IPlantApiService plantApiService,
            IOptionsMonitor<MainApiOptions> options)
        {
            _mainApiClient = mainApiClient;
            _plantApiService = plantApiService;
            _apiVersion = options.CurrentValue.ApiVersion;
            _baseAddress = new Uri(options.CurrentValue.BaseAddress);
        }

        public async Task<IList<string>> GetPermissionsAsync(string plant)
        {
            if (!await _plantApiService.IsPlantValidAsync(plant))
            {
                return new List<string>();
            }

            var url = $"{_baseAddress}Permissions" +
                      $"?plantId={plant}" +
                      $"&api-version={_apiVersion}";

            return await _mainApiClient.QueryAndDeserialize<List<string>>(url) ?? new List<string>();
        }

        public async Task<IList<string>> GetContentRestrictionsAsync(string plant)
        {
            if (!await _plantApiService.IsPlantValidAsync(plant))
            {
                return new List<string>();
            }

            var url = $"{_baseAddress}ContentRestrictions" +
                      $"?plantId={plant}" +
                      $"&api-version={_apiVersion}";

            return await _mainApiClient.QueryAndDeserialize<List<string>>(url) ?? new List<string>();
        }
    }
}
