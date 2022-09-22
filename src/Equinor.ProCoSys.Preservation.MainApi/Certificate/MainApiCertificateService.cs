using System;
using System.Net;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.MainApi.Client;
using Microsoft.Extensions.Options;

namespace Equinor.ProCoSys.Preservation.MainApi.Certificate
{
    public class MainApiCertificateService : ICertificateApiService
    {
        private readonly string _apiVersion;
        private readonly Uri _baseAddress;
        private readonly IBearerTokenApiClient _mainApiClient;

        public MainApiCertificateService(IBearerTokenApiClient mainApiClient,
            IOptionsSnapshot<MainApiOptions> options)
        {
            _mainApiClient = mainApiClient;
            _apiVersion = options.Value.ApiVersion;
            _baseAddress = new Uri(options.Value.BaseAddress);
        }

        public async Task<PCSCertificateTagsModel> TryGetCertificateTagsAsync(string plant, string projectName, string certificateNo, string certificateType)
        {
            var url = $"{_baseAddress}Certificate/Tags" +
                      $"?plantId={plant}" +
                      $"&projectName={WebUtility.UrlEncode(projectName)}" +
                      $"&certificateNo={WebUtility.UrlEncode(certificateNo)}" +
                      $"&certificateType={WebUtility.UrlEncode(certificateType)}" +
                      $"&api-version={_apiVersion}";

            return await _mainApiClient.TryQueryAndDeserializeAsync<PCSCertificateTagsModel>(url);
        }
    }
}
