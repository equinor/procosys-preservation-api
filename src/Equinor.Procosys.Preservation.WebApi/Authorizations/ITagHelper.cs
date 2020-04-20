using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.WebApi.Authorizations
{
    public interface ITagHelper
    {
        Task<string> GetProjectName(int tagId);
        Task<string> GetResponsibleCode(int tagId);
    }
}
