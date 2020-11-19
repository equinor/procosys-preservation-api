using System.Threading;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.Command.Validators.StepValidators
{
    public interface IStepValidator
    {
        Task<bool> ExistsAsync(int stepId, CancellationToken token);
        Task<bool> IsVoidedAsync(int stepId, CancellationToken token);
        Task<bool> IsAnyStepForSupplierAsync(int stepAId, int stepBId, CancellationToken token);
        Task<bool> IsFirstStepOrModeIsNotForSupplierAsync(int journeyId, int modeId, int stepId, CancellationToken token);
        Task<bool> IsForSupplierAsync(int stepId, CancellationToken token);
        Task<bool> HasModeAsync(int modeId, int stepId, CancellationToken token);
    }
}
