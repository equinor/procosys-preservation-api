using System.Threading.Tasks;

namespace Equinor.ProCoSys.Preservation.WebApi.Authentication
{
    public interface IApplicationAuthenticator
    {
        ValueTask<string> GetBearerTokenForApplicationAsync();
    }
}
