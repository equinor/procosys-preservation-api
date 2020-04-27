using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.WebApi.Misc
{
    public interface ITagHelper
    {
        Task<string> GetProjectNameAsync(int tagId);
        Task<string> GetResponsibleCodeAsync(int tagId);
    }
}
