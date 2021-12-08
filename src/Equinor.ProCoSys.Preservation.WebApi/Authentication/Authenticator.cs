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
    public class Authenticator : IBearerTokenProvider, IBearerTokenSetter, IAuthenticator
    {
        private readonly IOptionsMonitor<AuthenticatorOptions> _options;
        private readonly ILogger<Authenticator> _logger;
        private string _requestToken;
        private string _onBehalfOfUserToken;
        private string _applicationToken;
        private readonly string _secretInfo;

        public Authenticator(IOptionsMonitor<AuthenticatorOptions> options, ILogger<Authenticator> logger)
        {
            _options = options;
            _logger = logger;
            var apiSecret = _options.CurrentValue.PreservationApiSecret;
            _secretInfo = $"{apiSecret.Substring(0, 2)}***{apiSecret.Substring(apiSecret.Length - 1, 1)}";
            AuthenticationType = AuthenticationType.OnBehalfOf;
        }
        
        public AuthenticationType AuthenticationType { get; set; }

        public void SetBearerToken(string token) => _requestToken = token;

        public async ValueTask<string> GetBearerTokenAsync()
        {
            _logger.LogInformation($"Global setting=[{_options.CurrentValue.GlobalSetting}]");
            _logger.LogInformation($"Scoped setting=[{_options.CurrentValue.ScopedSetting}]");

            switch (AuthenticationType)
            {
                case AuthenticationType.OnBehalfOf:
                    return await GetBearerTokenOnBehalfOfCurrentUserAsync();
                case AuthenticationType.AsApplication:
                    return await GetBearerTokenForApplicationAsync();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async ValueTask<string> GetBearerTokenOnBehalfOfCurrentUserAsync()
        {
            if (_onBehalfOfUserToken == null)
            {
                if (string.IsNullOrEmpty(_requestToken))
                {
                    throw new ArgumentNullException(nameof(_requestToken));
                }

                var app = CreateConfidentialPreservationClient();

                var tokenResult = await app
                    .AcquireTokenOnBehalfOf(new List<string> { _options.CurrentValue.MainApiScope }, new UserAssertion(_requestToken))
                    .ExecuteAsync();
                _logger.LogInformation("Got token on behalf of");

                _onBehalfOfUserToken = tokenResult.AccessToken;
            }
            return _onBehalfOfUserToken;

        }

        private async ValueTask<string> GetBearerTokenForApplicationAsync()
        {
            if (_applicationToken == null)
            {
                var app = CreateConfidentialPreservationClient();

                var tokenResult = await app
                    .AcquireTokenForClient(new List<string> { _options.CurrentValue.MainApiScope })
                    .ExecuteAsync();
                _logger.LogInformation("Got token for application");

                _applicationToken = tokenResult.AccessToken;
            }
            return _applicationToken;
        }

        private IConfidentialClientApplication CreateConfidentialPreservationClient()
        {
            _logger.LogInformation($"Getting client using {_secretInfo} for {_options.CurrentValue.PreservationApiClientId}");
            return ConfidentialClientApplicationBuilder
                .Create(_options.CurrentValue.PreservationApiClientId)
                .WithClientSecret(_options.CurrentValue.PreservationApiSecret)
                .WithAuthority(new Uri(_options.CurrentValue.Instance))
                .Build();
        }
    }
}
