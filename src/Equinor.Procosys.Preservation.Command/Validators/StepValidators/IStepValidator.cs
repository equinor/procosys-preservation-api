﻿using System.Threading;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.Command.Validators.StepValidators
{
    public interface IStepValidator
    {
        Task<bool> ExistsAsync(int stepId, CancellationToken token);
        Task<bool> ExistsAsync(int journeyId, string stepTitle, CancellationToken token);
        Task<bool> ExistsInExistingJourneyAsync(int stepId, string stepTitle, CancellationToken token);
        Task<bool> IsVoidedAsync(int stepId, CancellationToken token);
        Task<bool> IsAnySupplierStep(int stepAId, int stepBId, CancellationToken token);
    }
}
