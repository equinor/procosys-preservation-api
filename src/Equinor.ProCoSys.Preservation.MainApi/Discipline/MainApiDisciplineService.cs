using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Auth.Client;
using Microsoft.Extensions.Options;

namespace Equinor.ProCoSys.Preservation.MainApi.Discipline
{
    public class MainApiDisciplineService : IDisciplineApiService
    {
        private readonly string _apiVersion;
        private readonly Uri _baseAddress;
        private readonly IMainApiClientForUser _mainApiClient;

        public MainApiDisciplineService(IMainApiClientForUser mainApiClient,
            IOptionsSnapshot<MainApiOptions> options)
        {
            _mainApiClient = mainApiClient;
            _apiVersion = options.Value.ApiVersion;
            _baseAddress = new Uri(options.Value.BaseAddress);
        }

        public async Task<PCSDiscipline> TryGetDisciplineAsync(string plant, string code, CancellationToken cancellationToken)
        {
            var url = $"{_baseAddress}Library/Discipline" +
                      $"?plantId={plant}" +
                      $"&code={WebUtility.UrlEncode(code)}" +
                      $"&api-version={_apiVersion}";

            return await _mainApiClient.TryQueryAndDeserializeAsync<PCSDiscipline>(url, cancellationToken);
        }
    }
}
