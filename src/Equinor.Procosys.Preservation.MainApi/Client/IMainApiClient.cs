using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.MainApi.Client
{
    public interface IMainApiClient
    {
        Task<T> QueryAndDeserialize<T>(string url);
    }
}
