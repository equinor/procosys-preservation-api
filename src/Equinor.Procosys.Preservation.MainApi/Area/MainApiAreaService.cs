using System;
using System.Net;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.MainApi.Client;
using Microsoft.Extensions.Options;

namespace Equinor.ProCoSys.Preservation.MainApi.Area
{
    public class MainApiAreaService : IAreaApiService
    {
        private readonly string _apiVersion;
        private readonly Uri _baseAddress;
        private readonly IBearerTokenApiClient _mainApiClient;

        public MainApiAreaService(IBearerTokenApiClient mainApiClient,
            IOptionsMonitor<MainApiOptions> options)
        {
            _mainApiClient = mainApiClient;
            _apiVersion = options.CurrentValue.ApiVersion;
            _baseAddress = new Uri(options.CurrentValue.BaseAddress);
        }

        public async Task<ProcosysArea> TryGetAreaAsync(string plant, string code)
        {
            var url = $"{_baseAddress}Library/Area" +
                      $"?plantId={plant}" +
                      $"&code={WebUtility.UrlEncode(code)}" +
                      $"&api-version={_apiVersion}";

            return await _mainApiClient.TryQueryAndDeserializeAsync<ProcosysArea>(url);
        }
    }
}
