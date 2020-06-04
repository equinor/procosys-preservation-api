using System.Threading;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.Command.Validators.ModeValidators
{
    public interface IModeValidator
    {
        Task<bool> ExistsAsync(int modeId, CancellationToken token);
        Task<bool> ExistsWithSameTitleAsync(string title, CancellationToken token);
        Task<bool> ExistsAnotherModeWithSameTitleAsync(int modeId, string title, CancellationToken token);
        Task<bool> IsVoidedAsync(int modeId, CancellationToken token);
        Task<bool> IsUsedInStepAsync(int modeId, CancellationToken token);
        Task<bool> ExistsAnotherModeTitleForSupplierAsync(string title, bool forSupplier, CancellationToken token);
        Task<bool> ExistsAnotherModeIdForSupplierAsync(int modeId, bool forSupplier, CancellationToken token);
    }
}
