using System;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.MainApi.Client;
using Microsoft.Extensions.Options;

namespace Equinor.ProCoSys.Preservation.MainApi.Person
{
    public class MainApiPersonService : IPersonApiService
    {
        private readonly Uri _baseAddress;
        private readonly string _apiVersion;
        private readonly IBearerTokenApiClient _mainApiClient;

        public MainApiPersonService(
            IBearerTokenApiClient mainApiClient,
            IOptionsMonitor<MainApiOptions> options)
        {
            _mainApiClient = mainApiClient;
            _baseAddress = new Uri(options.CurrentValue.BaseAddress);
            _apiVersion = options.CurrentValue.ApiVersion;
        }

        public async Task<PCSPerson> TryGetPersonByOidAsync(string azureOid)
        {
            var url = $"{_baseAddress}Person" +
                      $"?azureOid={azureOid}" +
                      $"&api-version={_apiVersion}";

            return await _mainApiClient.TryQueryAndDeserializeAsync<PCSPerson>(url);
        }
    }
}
