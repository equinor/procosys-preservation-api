﻿using System.Threading;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.Command.Validators.JourneyValidators
{
    public interface IJourneyValidator
    {
        Task<bool> ExistsAsync(int journeyId, CancellationToken token);
        Task<bool> StepExistsAsync(int journeyId, int stepId, CancellationToken token);
        Task<bool> ExistsWithSameTitleAsync(string journeyTitle, CancellationToken token);
        Task<bool> ExistsWithSameTitleInAnotherJourneyAsync(int journeyId, string journeyTitle, CancellationToken token);
        Task<bool> AnyStepExistsWithSameTitleAsync(int journeyId, string stepTitle, CancellationToken token); // todo tech move to journeyvalidator. Rename to StepExistsAsync
        Task<bool> OtherStepExistsWithSameTitleAsync(int journeyId, int stepId, string stepTitle, CancellationToken token); // todo tech move to journeyvalidator. Rename to StepExistsAsync. Add journeyId as param
        Task<bool> IsVoidedAsync(int journeyId, CancellationToken token);
        Task<bool> AreAdjacentStepsInAJourneyAsync(int journeyId, int stepAId, int stepBId, CancellationToken token);
        Task<bool> HasAnyStepsAsync(int journeyId, CancellationToken token);
        Task<bool> IsInUseAsync(int journeyId, CancellationToken token);
        Task<bool> ExistsWithDuplicateTitleAsync(int journeyId, CancellationToken token);
        Task<bool> HasAnyStepWithTransferOnRfccSignAsync(int journeyId, CancellationToken token);
        Task<bool> HasAnyStepWithTransferOnRfocSignAsync(int journeyId, CancellationToken token);
        Task<bool> HasOtherStepWithTransferOnRfccSignAsync(int journeyId, int stepId, CancellationToken token);
        Task<bool> HasOtherStepWithTransferOnRfocSignAsync(int journeyId, int stepId, CancellationToken token);
    }
}
