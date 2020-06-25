using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.MainApi.Client
{
    public interface IBearerTokenProvider
    {
        ValueTask<string> GetBearerTokenOnBehalfOfCurrentUserAsync();
    }
}
