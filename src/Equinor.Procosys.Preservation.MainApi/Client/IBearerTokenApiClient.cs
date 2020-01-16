using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.MainApi.Client
{
    public interface IBearerTokenApiClient
    {
        Task<T> QueryAndDeserialize<T>(string url);
    }
}
