using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.WebApi.Authentication
{
    public interface IApplicationAuthenticator
    {
        ValueTask<string> GetBearerTokenForApplicationAsync();
    }
}
