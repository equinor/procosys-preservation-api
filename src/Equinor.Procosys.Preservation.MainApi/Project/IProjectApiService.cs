using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.MainApi.Project
{
    public interface IProjectApiService
    {
        Task<ProcosysProject> TryGetProjectAsync(string plant, string name);
    }
}
