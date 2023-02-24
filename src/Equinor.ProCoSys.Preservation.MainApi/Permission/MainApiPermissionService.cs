using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.MainApi.Client;
using Microsoft.Extensions.Options;

namespace Equinor.ProCoSys.Preservation.MainApi.Permission
{
    public class MainApiPermissionService : IPermissionApiService
    {
        private readonly string _apiVersion;
        private readonly Uri _baseAddress;
        private readonly IBearerTokenApiClient _mainApiClient;

        public MainApiPermissionService(IBearerTokenApiClient mainApiClient,
            IOptionsSnapshot<MainApiOptions> options)
        {
            _mainApiClient = mainApiClient;
            _apiVersion = options.Value.ApiVersion;
            _baseAddress = new Uri(options.Value.BaseAddress);
        }
        
        public async Task<List<ProCoSysProject>> GetAllOpenProjectsAsync(string plantId)
        {
            var url = $"{_baseAddress}Projects" +
                      $"?plantId={plantId}" +
                      "&withCommPkgsOnly=false" +
                      "&includeProjectsWithoutAccess=true" +
                      $"&api-version={_apiVersion}";

            return await _mainApiClient.QueryAndDeserializeAsync<List<ProCoSysProject>>(url) ?? new List<ProCoSysProject>();
        }

        public async Task<List<string>> GetPermissionsAsync(string plantId)
        {
            var url = $"{_baseAddress}Permissions" +
                      $"?plantId={plantId}" +
                      $"&api-version={_apiVersion}";

            return await _mainApiClient.QueryAndDeserializeAsync<List<string>>(url) ?? new List<string>();
        }

        public async Task<List<string>> GetContentRestrictionsAsync(string plantId)
        {
            var url = $"{_baseAddress}ContentRestrictions" +
                      $"?plantId={plantId}" +
                      $"&api-version={_apiVersion}";

            return await _mainApiClient.QueryAndDeserializeAsync<List<string>>(url) ?? new List<string>();
        }
    }
}
