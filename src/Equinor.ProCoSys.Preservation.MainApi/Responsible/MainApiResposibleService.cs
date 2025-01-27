using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Auth.Client;
using Microsoft.Extensions.Options;

namespace Equinor.ProCoSys.Preservation.MainApi.Responsible
{
    public class MainApiResponsibleService : IResponsibleApiService
    {
        private readonly string _apiVersion;
        private readonly Uri _baseAddress;
        private readonly IMainApiClientForUser _mainApiClient;

        public MainApiResponsibleService(IMainApiClientForUser mainApiClient,
            IOptionsSnapshot<MainApiOptions> options)
        {
            _mainApiClient = mainApiClient;
            _apiVersion = options.Value.ApiVersion;
            _baseAddress = new Uri(options.Value.BaseAddress);
        }

        public async Task<PCSResponsible> TryGetResponsibleAsync(string plant, string code, CancellationToken cancellationToken)
        {
            var url = $"{_baseAddress}Library/Responsible" +
                      $"?plantId={plant}" +
                      $"&code={WebUtility.UrlEncode(code)}" +
                      $"&api-version={_apiVersion}";

            return await _mainApiClient.TryQueryAndDeserializeAsync<PCSResponsible>(url, cancellationToken);
        }
    }
}
