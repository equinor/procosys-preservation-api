using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.MainApi.Client;
using Equinor.Procosys.Preservation.WebApi.Misc;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;

namespace Equinor.Procosys.Preservation.WebApi.Authentication
{
    public class Authenticator : IBearerTokenProvider, IBearerTokenSetter, IApplicationAuthenticator
    {
        private readonly IOptions<AuthenticatorOptions> _options;
        private bool _canUseOnBehalfOf;
        private string _requestToken;
        private AuthenticationResult _onBehalfOfUserToken;
        private AuthenticationResult _applicationToken;

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
                if (_onBehalfOfUserToken == null || _onBehalfOfUserToken.ExpiresOn < TimeService.UtcNow.AddMinutes(10))
                {
                    var app = ConfidentialClientApplicationBuilder
                        .Create(_options.Value.PreservationApiClientId)
                        .WithClientSecret(_options.Value.PreservationApiSecret)
                        .WithAuthority(new Uri(_options.Value.Instance))
                        .Build();

                    var tokenResult = await app
                        .AcquireTokenOnBehalfOf(new List<string> { _options.Value.MainApiScope }, new UserAssertion(_requestToken.ToString()))
                        .ExecuteAsync();

                    _onBehalfOfUserToken = tokenResult;
                }
                return _onBehalfOfUserToken.AccessToken;
            }
            else
            {
                return _requestToken;
            }
        }

        public async ValueTask<string> GetBearerTokenForApplicationAsync()
        {
            if (_applicationToken == null || _applicationToken.ExpiresOn < TimeService.UtcNow.AddMinutes(10))
            {
                var app = ConfidentialClientApplicationBuilder
                    .Create(_options.Value.MainApiClientId)
                    .WithClientSecret(_options.Value.MainApiSecret)
                    .WithAuthority(new Uri(_options.Value.Instance))
                    .Build();

                var tokenResult = await app
                    .AcquireTokenForClient(new List<string> { _options.Value.MainApiScope })
                    .ExecuteAsync();

                _applicationToken = tokenResult;
            }
            return _applicationToken.AccessToken;
        }
    }
}
