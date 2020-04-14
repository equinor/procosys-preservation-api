using System.Collections.Generic;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.MainApi.Project
{
    public interface IProjectApiService
    {
        Task<IList<ProcosysProject>> GetProjectsAsync(string plant);
        Task<ProcosysProject> GetProjectAsync(string plant, string name);
    }
}
