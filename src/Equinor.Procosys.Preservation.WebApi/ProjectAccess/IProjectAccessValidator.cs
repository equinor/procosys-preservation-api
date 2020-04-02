using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.WebApi.ProjectAccess
{
    public interface IProjectAccessValidator
    {
        Task<bool> ValidateAsync(object request);
    }
}
