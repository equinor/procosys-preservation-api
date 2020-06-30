using Equinor.Procosys.Preservation.MainApi.Client;

namespace Equinor.Procosys.Preservation.WebApi.Misc
{
    public class RequestBearerTokenProvider : IBearerTokenProvider, IBearerTokenSetter
    {
        private string _token;

        public string GetBearerToken() => _token;

        public void SetBearerToken(string token) => _token = token;
    }
}
