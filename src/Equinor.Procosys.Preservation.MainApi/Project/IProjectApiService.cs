using System.Threading.Tasks;

namespace Equinor.ProCoSys.Preservation.MainApi.Project
{
    public interface IProjectApiService
    {
        Task<PCSProject> TryGetProjectAsync(string plant, string name);
    }
}
