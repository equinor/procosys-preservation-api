using System.Threading;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.Command.Validators.SavedFilterValidators
{
    public interface ISavedFilterValidator
    {
        Task<bool> ExistsWithSameTitleForPersonInProjectAsync(string title, string projectName, CancellationToken cancellationToken);
        Task<bool> ExistsWithSameTitleForPersonInProjectAsync(int savedFilterId, string title, CancellationToken cancellationToken);
        Task<bool> ExistsAsync(int savedFilterId, CancellationToken token);
    }
}
