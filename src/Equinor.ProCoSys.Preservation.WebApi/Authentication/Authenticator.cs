using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.MainApi.Client;
using Equinor.ProCoSys.Preservation.WebApi.Misc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;

namespace Equinor.ProCoSys.Preservation.WebApi.Authentication
{
    public class Authenticator : IBearerTokenProvider, IBearerTokenSetter, IApplicationAuthenticator
    {
        private readonly IOptions<AuthenticatorOptions> _options;
        private readonly ILogger<BearerTokenApiClient> _logger;
        private bool _canUseOnBehalfOf;
        private string _requestToken;
        private string _onBehalfOfUserToken;
        private string _applicationToken;

        public Authenticator(IOptions<AuthenticatorOptions> options, ILogger<BearerTokenApiClient> logger)
        {
            _options = options;
            _logger = logger;
        }

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
                    var apiSecret = _options.Value.PreservationApiSecret;
                    var secret = $"{apiSecret.Substring(0, 3)}***{apiSecret.Substring(apiSecret.Length - 3, 3)}";
                    _logger.LogInformation($"Getting onbehalf of token using {secret} for {_options.Value.PreservationApiClientId}");
                    var app = CreateConfidentialPreservationClient();

                    var tokenResult = await app
                        .AcquireTokenOnBehalfOf(new List<string> { _options.Value.MainApiScope }, new UserAssertion(_requestToken))
                        .ExecuteAsync();
                    _logger.LogInformation("Got onbehalf of token");

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
                var app = CreateConfidentialPreservationClient();

                var tokenResult = await app
                    .AcquireTokenForClient(new List<string> { _options.Value.MainApiScope })
                    .ExecuteAsync();

                _applicationToken = tokenResult.AccessToken;
            }
            return _applicationToken;
        }

        private IConfidentialClientApplication CreateConfidentialPreservationClient()
            => ConfidentialClientApplicationBuilder
                .Create(_options.Value.PreservationApiClientId)
                .WithClientSecret(_options.Value.PreservationApiSecret)
                .WithAuthority(new Uri(_options.Value.Instance))
                .Build();
    }
}
