using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.MainApi.Client;
using Microsoft.Extensions.Options;

namespace Equinor.ProCoSys.Preservation.MainApi.Plant
{
    public class MainApiPlantService : IPlantApiService
    {
        private readonly IAuthenticator _authenticator;
        private readonly string _apiVersion;
        private readonly Uri _baseAddress;
        private readonly IBearerTokenApiClient _mainApiClient;

        public MainApiPlantService(
            IAuthenticator authenticator,
            IBearerTokenApiClient mainApiClient,
            IOptionsSnapshot<MainApiOptions> options)
        {
            _authenticator = authenticator;
            _mainApiClient = mainApiClient;
            _apiVersion = options.Value.ApiVersion;
            _baseAddress = new Uri(options.Value.BaseAddress);
        }

        public async Task<List<PCSPlant>> GetAllPlantsForUserAsync(Guid azureOid)
        {
            var url = $"{_baseAddress}Plants/ForUser" +
                      $"?azureOid={azureOid:D}" +
                      "&includePlantsWithoutAccess=true" +
                      $"&api-version={_apiVersion}";

            var oldAuthType = _authenticator.AuthenticationType;
            _authenticator.AuthenticationType = AuthenticationType.AsApplication;
            try
            {
                return await _mainApiClient.QueryAndDeserializeAsync<List<PCSPlant>>(url);
            }
            finally
            {
                _authenticator.AuthenticationType = oldAuthType;
            }
        }
    }
}
