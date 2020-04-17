using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.WebApi.Authorizations
{
    public interface IProjectHelper
    {
        Task<string> GetProjectNameFromTagIdAsync(int tagId);
    }
}
