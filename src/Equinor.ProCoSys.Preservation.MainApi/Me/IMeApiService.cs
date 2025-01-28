using System.Threading;
using System.Threading.Tasks;

namespace Equinor.ProCoSys.Preservation.MainApi.Me
{
    public interface IMeApiService
    {
        Task TracePlantAsync(string plant, CancellationToken cancellationToken);
    }
}
