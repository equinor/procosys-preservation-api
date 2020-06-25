using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.MainApi.Client;
using Equinor.Procosys.Preservation.WebApi.Misc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;

namespace Equinor.Procosys.Preservation.WebApi.Authentication
{
    public class TokenProvider : IBearerTokenProvider, IBearerTokenSetter, IApplicationAuthenticator
    {
        private readonly IOptions<AuthenticatorOptions> _options;
        private string _token;

        public TokenProvider(IOptions<AuthenticatorOptions> options)
        {
            _options = options;
        }

        public void SetBearerToken(string token) => _token = token;

        public async ValueTask<string> GetBearerTokenOnBehalfOfCurrentUserAsync()
        {
            var app = ConfidentialClientApplicationBuilder
                .Create(_options.Value.ClientId)
                .WithClientSecret(_options.Value.ClientSecret)
                .WithAuthority(new Uri(_options.Value.Instance))
                .Build();

            var tokenResult = await app
                .AcquireTokenOnBehalfOf(new List<string> { _options.Value.MainApiScope }, new UserAssertion(_token))
                .ExecuteAsync();

            return tokenResult.AccessToken;
        }

        public async ValueTask<string> GetBearerTokenForApplicationAsync()
        {
            var app = ConfidentialClientApplicationBuilder.Create(_options.Value.ClientId)
           .WithClientSecret(_options.Value.ClientSecret)
           .WithAuthority(new Uri(_options.Value.Instance))
           .Build();

            var tokenResult = await app.AcquireTokenForClient(new List<string> { _options.Value.PreservationApiScope }).ExecuteAsync();
            return tokenResult.AccessToken;
        }
    }
}
