using System;
using System.Collections.Generic;
using Equinor.ProCoSys.Auth.Authentication;
using Microsoft.Extensions.Options;

namespace Equinor.ProCoSys.Preservation.WebApi.Authentication
{
    public class AuthenticatorOptions : IAuthenticatorOptions
    {
        protected readonly IOptionsMonitor<PreservationAuthenticatorOptions> _options;

        private readonly IDictionary<string, string> _scopes = new Dictionary<string, string>();
        
        public AuthenticatorOptions(IOptionsMonitor<PreservationAuthenticatorOptions> options)
        {
            _options = options;
            _scopes.Add("MainApiScope", _options.CurrentValue.MainApiScope);
        }

        public string Instance => _options.CurrentValue.Instance;

        public string ClientId => _options.CurrentValue.PreservationApiClientId;

        public string Secret => _options.CurrentValue.PreservationApiSecret;

        public Guid ObjectId => _options.CurrentValue.PreservationApiObjectId;

        public IDictionary<string, string> Scopes => _scopes;
    }
}
