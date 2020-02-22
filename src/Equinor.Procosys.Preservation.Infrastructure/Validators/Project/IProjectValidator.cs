using System.Threading;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.Infrastructure.Validators.Project
{
    public interface IProjectValidator
    {
        Task<bool> ExistsAsync(string projectName, CancellationToken cancellationToken);
        
        Task<bool> IsExistingAndClosedAsync(string projectName, CancellationToken cancellationToken);
        
        Task<bool> IsClosedForTagAsync(int tagId, CancellationToken cancellationToken);
    }
}
