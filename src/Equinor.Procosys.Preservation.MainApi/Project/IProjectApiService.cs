using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.MainApi.Project
{
    public interface IProjectApiService
    {
        Task<ProcosysProject> GetProjectAsync(string plant, string name);
    }
}
