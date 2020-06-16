using System.Net.Http;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.MainApi.Client
{
    public interface IBearerTokenApiClient
    {
        Task<T> QueryAndDeserialize<T>(string url); // todo rename to QueryAndDeserializeAsync
        Task PutAsync(string url, HttpContent content);
    }
}
