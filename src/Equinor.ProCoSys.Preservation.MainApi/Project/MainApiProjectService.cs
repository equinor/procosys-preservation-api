using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Auth.Client;
using Microsoft.Extensions.Options;

namespace Equinor.ProCoSys.Preservation.MainApi.Project
{
    public abstract class MainApiProjectService(
        IApiClient mainApiClient,
        IOptionsSnapshot<MainApiOptions> options)
    {
        private readonly string _apiVersion = options.Value.ApiVersion;
        private readonly Uri _baseAddress = new(options.Value.BaseAddress);

        public async Task<ProCoSysProject> TryGetProjectAsync(string plant, string name, CancellationToken cancellationToken)
        {
            var url = $"{_baseAddress}ProjectByName" +
                $"?plantId={plant}" +
                $"&projectName={WebUtility.UrlEncode(name)}" +
                $"&api-version={_apiVersion}";

            return await mainApiClient.TryQueryAndDeserializeAsync<ProCoSysProject>(url, cancellationToken);
        }
    }
}
