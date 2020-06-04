using System.Threading;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.Command.Validators.JourneyValidators
{
    public interface IJourneyValidator
    {
        Task<bool> ExistsAsync(int journeyId, CancellationToken token);
        Task<bool> ExistsWithSameTitleAsync(string journeyTitle, CancellationToken token);
        Task<bool> ExistsWithSameTitleInAnotherJourneyAsync(int journeyId, string journeyTitle, CancellationToken token);
        Task<bool> IsVoidedAsync(int journeyId, CancellationToken token);
        Task<bool> AreAdjacentStepsInAJourneyAsync(int journeyId, int stepAId, int stepBId, CancellationToken token);
        Task<bool> IsFirstStepInAJourneyIfSupplierStepAsync(int journeyId, int modeId, CancellationToken token);
    }
}
