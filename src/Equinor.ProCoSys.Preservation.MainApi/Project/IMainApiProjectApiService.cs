using System.Threading;
using System.Threading.Tasks;

namespace Equinor.ProCoSys.Preservation.MainApi.Project
{
    public interface IMainApiProjectApiService
    {
        Task<ProCoSysProject> TryGetProjectAsync(string plant, string name, CancellationToken cancellationToken);
    }
}
