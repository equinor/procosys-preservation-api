using System.Threading;
using System.Threading.Tasks;

namespace Equinor.ProCoSys.Preservation.MainApi.Area
{
    public interface IAreaApiService
    {
        Task<PCSArea> TryGetAreaAsync(string plant, string code, CancellationToken cancellationToken);
    }
}
