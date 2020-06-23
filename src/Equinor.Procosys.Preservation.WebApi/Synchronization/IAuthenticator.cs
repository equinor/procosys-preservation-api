using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.WebApi.Synchronization
{
    public interface IAuthenticator
    {
        Task<string> GetBearerTokenAsync();
    }
}
