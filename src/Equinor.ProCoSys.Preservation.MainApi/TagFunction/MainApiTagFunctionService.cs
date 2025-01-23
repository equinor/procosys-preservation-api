using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Auth.Client;
using Microsoft.Extensions.Options;

namespace Equinor.ProCoSys.Preservation.MainApi.TagFunction
{
    public class MainApiTagFunctionService : ITagFunctionApiService
    {
        private readonly string _apiVersion;
        private readonly Uri _baseAddress;
        private readonly IMainApiClientForApplication _mainApiClient;

        public MainApiTagFunctionService(IMainApiClientForApplication mainApiClient,
            IOptionsSnapshot<MainApiOptions> options)
        {
            _mainApiClient = mainApiClient;
            _apiVersion = options.Value.ApiVersion;
            _baseAddress = new Uri(options.Value.BaseAddress);
        }

        public async Task<PCSTagFunction> TryGetTagFunctionAsync(
            string plant,
            string tagFunctionCode,
            string registerCode,
            CancellationToken cancellationToken)
        {
            var url = $"{_baseAddress}Library/TagFunction" +
                $"?plantId={plant}" +
                $"&tagFunctionCode={WebUtility.UrlEncode(tagFunctionCode)}" +
                $"&registerCode={WebUtility.UrlEncode(registerCode)}" +
                $"&api-version={_apiVersion}";

            return await _mainApiClient.TryQueryAndDeserializeAsync<PCSTagFunction>(url, cancellationToken);
        }
    }
}
