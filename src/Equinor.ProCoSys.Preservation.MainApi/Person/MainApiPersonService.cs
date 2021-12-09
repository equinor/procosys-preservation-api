using System;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.MainApi.Client;
using Microsoft.Extensions.Options;

namespace Equinor.ProCoSys.Preservation.MainApi.Person
{
    public class MainApiPersonService : IPersonApiService
    {
        private readonly IAuthenticator _authenticator;
        private readonly Uri _baseAddress;
        private readonly string _apiVersion;
        private readonly IBearerTokenApiClient _mainApiClient;

        public MainApiPersonService(
            IAuthenticator authenticator,
            IBearerTokenApiClient mainApiClient,
            IOptionsSnapshot<MainApiOptions> options)
        {
            _authenticator = authenticator;
            _mainApiClient = mainApiClient;
            _baseAddress = new Uri(options.Value.BaseAddress);
            _apiVersion = options.Value.ApiVersion;
        }

        public async Task<PCSPerson> TryGetPersonByOidAsync(Guid azureOid)
        {
            var url = $"{_baseAddress}Person" +
                      $"?azureOid={azureOid:D}" +
                      $"&api-version={_apiVersion}";

            var oldAuthType = _authenticator.AuthenticationType;
            _authenticator.AuthenticationType = AuthenticationType.AsApplication;
            try
            {
                return await _mainApiClient.TryQueryAndDeserializeAsync<PCSPerson>(url);
            }
            finally
            {
                _authenticator.AuthenticationType = oldAuthType;
            }
        }
    }
}
