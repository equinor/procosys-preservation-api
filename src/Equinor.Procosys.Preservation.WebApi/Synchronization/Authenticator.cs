using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;

namespace Equinor.Procosys.Preservation.WebApi.Synchronization
{
    public class Authenticator : IAuthenticator
    {
        private readonly IOptions<AuthenticatorOptions> _options;

        public Authenticator(IOptions<AuthenticatorOptions> options) => _options = options;

        public async Task<string> GetBearerTokenAsync()
        {
            var app = ConfidentialClientApplicationBuilder.Create(_options.Value.ClientId)
           .WithClientSecret(_options.Value.ClientSecret)
           .WithAuthority(new Uri(_options.Value.Instance))
           .Build();

            var tokenResult = await app.AcquireTokenForClient(_options.Value.Scopes).ExecuteAsync();
            return tokenResult.AccessToken;
        }
    }
}
