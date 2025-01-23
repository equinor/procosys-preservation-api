using System.Threading;
using System.Threading.Tasks;

namespace Equinor.ProCoSys.Preservation.MainApi.Responsible
{
    public interface IResponsibleApiService
    {
        Task<PCSResponsible> TryGetResponsibleAsync(string plant, string code, CancellationToken cancellationToken);
    }
}
