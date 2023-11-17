using System.Threading;
using System.Threading.Tasks;

namespace Equinor.ProCoSys.Preservation.Command.Validators.ModeValidators
{
    public interface IModeValidator
    {
        Task<bool> ExistsAsync(int modeId, CancellationToken token);
        Task<bool> ExistsWithSameTitleAsync(string title, CancellationToken token);
        Task<bool> ExistsAnotherModeWithSameTitleAsync(int modeId, string title, CancellationToken token);
        Task<bool> IsVoidedAsync(int modeId, CancellationToken token);
        Task<bool> IsUsedInStepAsync(int modeId, CancellationToken token);
        Task<bool> ExistsAnyModeWithForSupplierAsync(CancellationToken token);
        Task<bool> ExistsAnotherModeForSupplierAsync(int modeId, CancellationToken token);
        Task<bool> ExistsWithForSupplierValueAsync(int modeId, bool forSupplier, CancellationToken token);
    }
}
