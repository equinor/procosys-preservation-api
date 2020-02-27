using System.Threading;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.Command.Validators.ModeValidators
{
    public interface IModeValidator
    {
        Task<bool> ExistsAsync(int modeId, CancellationToken token);
        Task<bool> ExistsAsync(string title, CancellationToken token);
        Task<bool> IsVoidedAsync(int modeId, CancellationToken token);
        Task<bool> IsUsedInStepAsync(int modeId, CancellationToken token);
    }
}
