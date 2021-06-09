using System.Threading;
using System.Threading.Tasks;

namespace Equinor.ProCoSys.Preservation.Command.Validators.StepValidators
{
    public interface IStepValidator
    {
        Task<bool> ExistsAsync(int stepId, CancellationToken token);
        Task<bool> IsVoidedAsync(int stepId, CancellationToken token);
        Task<bool> IsForSupplierAsync(int stepId, CancellationToken token);
        Task<bool> HasModeAsync(int modeId, int stepId, CancellationToken token);
    }
}
