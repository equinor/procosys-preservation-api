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
        private readonly string _secretInfo;

        public Authenticator(IOptions<AuthenticatorOptions> options, ILogger<BearerTokenApiClient> logger)
        {
            _options = options;
            _logger = logger;
            var apiSecret = _options.Value.PreservationApiSecret;
            _secretInfo = $"{apiSecret.Substring(0, 2)}***{apiSecret.Substring(apiSecret.Length - 1, 1)}";
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
                    _logger.LogInformation($"Global setting=[{_options.Value.GlobalSetting}]");
                    _logger.LogInformation($"Scoped setting=[{_options.Value.ScopedSetting}]");
                    var app = CreateConfidentialPreservationClient();

                    var tokenResult = await app
                        .AcquireTokenOnBehalfOf(new List<string> { _options.Value.MainApiScope }, new UserAssertion(_requestToken))
                        .ExecuteAsync();
                    _logger.LogInformation("Got token on behalf of");

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
                _logger.LogInformation("Got token for client");

                _applicationToken = tokenResult.AccessToken;
            }
            return _applicationToken;
        }

        private IConfidentialClientApplication CreateConfidentialPreservationClient()
        {
            _logger.LogInformation($"Getting client using {_secretInfo} for {_options.Value.PreservationApiClientId}");
            return ConfidentialClientApplicationBuilder
                .Create(_options.Value.PreservationApiClientId)
                .WithClientSecret(_options.Value.PreservationApiSecret)
                .WithAuthority(new Uri(_options.Value.Instance))
                .Build();
        }
    }
}
