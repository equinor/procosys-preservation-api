using Equinor.ProCoSys.Auth.Authentication;
using Equinor.ProCoSys.Preservation.WebApi.Authentication;
using Microsoft.Extensions.Options;

namespace Equinor.ProCoSys.Preservation.WebApi.Misc
{
    public class DebugConfigValues : IDebugConfigValues
    {
        protected readonly IOptionsMonitor<PreservationAuthenticatorOptions> _optionsFromMonitor;
        protected readonly IAuthenticatorOptions _options;

        public DebugConfigValues(
            IOptionsMonitor<PreservationAuthenticatorOptions> optionsFromMonitor,
            IAuthenticatorOptions options)
        {
            _optionsFromMonitor = optionsFromMonitor;
            _options = options;
        }

        public string GetValues()
            => $"From monitor: {ScrambleSecret(_optionsFromMonitor.CurrentValue.PreservationApiSecret)}. From interface: {ScrambleSecret(_options.Secret)}";

        private static string ScrambleSecret(string secret)
            => $"{secret.Substring(0, 2)}***{secret.Substring(secret.Length - 1, 1)}";
    }
}
