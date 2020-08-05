using System.Threading;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.Command.Validators.SavedFilterValidators
{
    public interface ISavedFilterValidator
    {
        Task<bool> ExistsWithSameTitleForPersonAsync(string title, CancellationToken cancellationToken);
    }
}
