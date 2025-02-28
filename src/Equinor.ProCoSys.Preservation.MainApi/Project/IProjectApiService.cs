using System.Threading;
using System.Threading.Tasks;

namespace Equinor.ProCoSys.Preservation.MainApi.Project
{
    public interface IMainApiProjectApiForUserService
    {
        Task<ProCoSysProject> TryGetProjectAsync(string plant, string name, CancellationToken cancellationToken);
    }
}
