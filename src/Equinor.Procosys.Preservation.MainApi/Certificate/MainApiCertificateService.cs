using System;
using System.Collections.Generic;
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
            IOptionsMonitor<MainApiOptions> options)
        {
            _mainApiClient = mainApiClient;
            _apiVersion = options.CurrentValue.ApiVersion;
            _baseAddress = new Uri(options.CurrentValue.BaseAddress);
        }

        public async Task<ProcosysCertificateTagsModel> TryGetCertificateTagsAsync(string plant, string projectName, string certificateNo, string certificateType)
        {
            var url = $"{_baseAddress}Certificate/Tags" +
                      $"?plantId={plant}" +
                      $"&projectName={WebUtility.UrlEncode(projectName)}" +
                      $"&certificateNo={WebUtility.UrlEncode(certificateNo)}" +
                      $"&certificateType={WebUtility.UrlEncode(certificateType)}" +
                      $"&api-version={_apiVersion}";

            return await _mainApiClient.TryQueryAndDeserializeAsync<ProcosysCertificateTagsModel>(url);
        }

        public async Task<IEnumerable<ProcosysCertificateModel>> GetAcceptedCertificatesAsync(string plant, DateTime cutoffAcceptedTime)
        {
            var url = $"{_baseAddress}Certificate/Accepted" +
                      $"?plantId={plant}" +
                      $"&cutoffAcceptedTime={cutoffAcceptedTime:O}" +
                      $"&api-version={_apiVersion}";

            return await _mainApiClient.QueryAndDeserializeAsync<List<ProcosysCertificateModel>>(url);
        }
    }
}
