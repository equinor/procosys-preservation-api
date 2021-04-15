using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.MainApi.Client;
using Equinor.ProCoSys.Preservation.WebApi.Misc;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;

namespace Equinor.ProCoSys.Preservation.WebApi.Authentication
{
    public class Authenticator : IBearerTokenProvider, IBearerTokenSetter, IApplicationAuthenticator
    {
        private readonly IOptions<AuthenticatorOptions> _options;
        private bool _canUseOnBehalfOf;
        private string _requestToken;
        private string _onBehalfOfUserToken;
        private string _applicationToken;

        public Authenticator(IOptions<AuthenticatorOptions> options) => _options = options;

        public void SetBearerToken(string token, bool isUserToken = true)
        {
            _requestToken = token ?? throw new ArgumentNullException(nameof(token));
            _canUseOnBehalfOf = isUserToken;
        }

        public async ValueTask<string> GetBearerTokenOnBehalfOfCurrentUserAsync()
        {
            if (_canUseOnBehalfOf)
            {
                if (_onBehalfOfUserToken == null)
                {
                    var app = ConfidentialClientApplicationBuilder
                        .Create(_options.Value.PreservationApiClientId)
                        .WithClientSecret(_options.Value.PreservationApiSecret)
                        .WithAuthority(new Uri(_options.Value.Instance))
                        .Build();

                    var tokenResult = await app
                        .AcquireTokenOnBehalfOf(new List<string> { _options.Value.MainApiScope }, new UserAssertion(_requestToken))
                        .ExecuteAsync();

                    _onBehalfOfUserToken = tokenResult.AccessToken;
                }
                return _onBehalfOfUserToken;
            }

            return _requestToken;
        }

        public async ValueTask<string> GetBearerTokenForApplicationAsync()
        {
            if (_applicationToken == null)
            {
                var app = ConfidentialClientApplicationBuilder
                    .Create(_options.Value.MainApiClientId)
                    .WithClientSecret(_options.Value.MainApiSecret)
                    .WithAuthority(new Uri(_options.Value.Instance))
                    .Build();

                var tokenResult = await app
                    .AcquireTokenForClient(new List<string> { _options.Value.MainApiScope })
                    .ExecuteAsync();

                _applicationToken = tokenResult.AccessToken;
            }
            return _applicationToken;
        }
    }
}
