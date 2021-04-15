using System;
using System.Net;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.MainApi.Client;
using Microsoft.Extensions.Options;

namespace Equinor.ProCoSys.Preservation.MainApi.TagFunction
{
    public class MainApiTagFunctionService : ITagFunctionApiService
    {
        private readonly string _apiVersion;
        private readonly Uri _baseAddress;
        private readonly IBearerTokenApiClient _mainApiClient;

        public MainApiTagFunctionService(IBearerTokenApiClient mainApiClient,
            IOptionsMonitor<MainApiOptions> options)
        {
            _mainApiClient = mainApiClient;
            _apiVersion = options.CurrentValue.ApiVersion;
            _baseAddress = new Uri(options.CurrentValue.BaseAddress);
        }

        public async Task<PCSTagFunction> TryGetTagFunctionAsync(string plant, string tagFunctionCode, string registerCode)
        {
            var url = $"{_baseAddress}Library/TagFunction" +
                $"?plantId={plant}" +
                $"&tagFunctionCode={WebUtility.UrlEncode(tagFunctionCode)}" +
                $"&registerCode={WebUtility.UrlEncode(registerCode)}" +
                $"&api-version={_apiVersion}";

            return await _mainApiClient.TryQueryAndDeserializeAsync<PCSTagFunction>(url);
        }
    }
}
