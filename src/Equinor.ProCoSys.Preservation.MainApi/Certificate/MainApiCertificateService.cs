using System;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Auth.Client;
using Microsoft.Extensions.Options;

namespace Equinor.ProCoSys.Preservation.MainApi.Certificate
{
    public class MainApiCertificateService : ICertificateApiService
    {
        private readonly string _apiVersion;
        private readonly Uri _baseAddress;
        private readonly IMainApiClientForUser _mainApiClient;

        public MainApiCertificateService(IMainApiClientForUser mainApiClient,
            IOptionsSnapshot<MainApiOptions> options)
        {
            _mainApiClient = mainApiClient;
            _apiVersion = options.Value.ApiVersion;
            _baseAddress = new Uri(options.Value.BaseAddress);
        }

        public async Task<PCSCertificateTagsModel> TryGetCertificateTagsAsync(string plant, Guid proCoSysGuid, CancellationToken cancellationToken)
        {
            var url = $"{_baseAddress}Certificate/TagsByCertificateGuid" +
                      $"?plantId={plant}" +
                      $"&proCoSysGuid={proCoSysGuid.ToString("N")}" +
                      $"&api-version={_apiVersion}";

            return await _mainApiClient.TryQueryAndDeserializeAsync<PCSCertificateTagsModel>(url, cancellationToken);
        }
    }
}
