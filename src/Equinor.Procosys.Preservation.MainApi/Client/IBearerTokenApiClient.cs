using System.Net.Http;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.MainApi.Client
{
    public interface IBearerTokenApiClient
    {
        Task<T> QueryAndDeserializeAsync<T>(string url);
        Task PutAsync(string url, HttpContent content);
    }
}
