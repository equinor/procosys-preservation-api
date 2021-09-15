using System.Threading.Tasks;

namespace Equinor.ProCoSys.Preservation.MainApi.Client
{
    public interface IBearerTokenProvider
    {
        ValueTask<string> GetBearerTokenAsync();
    }
}
