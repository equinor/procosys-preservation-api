using System.Threading.Tasks;

namespace Equinor.ProCoSys.Preservation.WebApi.ServiceBus.Misc
{
    public interface ITagHelper
    {
        Task<string> GetProjectNameAsync(int tagId);
        Task<string> GetResponsibleCodeAsync(int tagId);
    }
}
