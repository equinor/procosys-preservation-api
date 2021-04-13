using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Equinor.ProCoSys.Preservation.Command.Validators.ProjectValidators
{
    public interface IProjectValidator
    {
        Task<bool> ExistsAsync(string projectName, CancellationToken token);
        
        Task<bool> IsExistingAndClosedAsync(string projectName, CancellationToken token);
        
        Task<bool> IsClosedForTagAsync(int tagId, CancellationToken token);

        Task<bool> AllTagsInSameProjectAsync(IEnumerable<int> tagIds, CancellationToken token);
    }
}
