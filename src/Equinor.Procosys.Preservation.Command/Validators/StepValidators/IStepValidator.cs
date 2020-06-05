using System.Threading;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.Command.Validators.StepValidators
{
    public interface IStepValidator
    {
        Task<bool> ExistsAsync(int stepId, CancellationToken token);
        Task<bool> ExistsAsync(int journeyId, string stepTitle, CancellationToken token);
        Task<bool> ExistsInExistingJourneyAsync(int stepId, string stepTitle, CancellationToken token);
        Task<bool> IsVoidedAsync(int stepId, CancellationToken token);
        Task<bool> IsAnyStepForSupplier(int stepAId, int stepBId, CancellationToken token);
        Task<bool> IsFirstStepOrModeIsNotForSupplier(int journeyId, int modeId, int stepId,
            CancellationToken token);
        Task<bool> IsForSupplierAsync(int stepId, CancellationToken token);
    }
}
