﻿using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;

namespace Equinor.ProCoSys.Preservation.Command.Validators.JourneyValidators
{
    public interface IJourneyValidator
    {
        Task<bool> ExistsAsync(int journeyId, CancellationToken token);
        Task<bool> ExistsStepAsync(int journeyId, int stepId, CancellationToken token);
        Task<bool> ExistsWithSameTitleAsync(string journeyTitle, CancellationToken token);
        Task<bool> ExistsWithSameTitleInAnotherJourneyAsync(int journeyId, string journeyTitle, CancellationToken token);
        Task<bool> AnyStepExistsWithSameTitleAsync(int journeyId, string stepTitle, CancellationToken token);
        Task<bool> OtherStepExistsWithSameTitleAsync(int journeyId, int stepId, string stepTitle, CancellationToken token);
        Task<bool> IsVoidedAsync(int journeyId, CancellationToken token);
        Task<bool> AreAdjacentStepsInAJourneyAsync(int journeyId, int stepAId, int stepBId, CancellationToken token);
        Task<bool> HasAnyStepsAsync(int journeyId, CancellationToken token);
        Task<bool> HasAnyStepInJourneyATagAsync(int journeyId, CancellationToken token);
        Task<bool> ExistsWithDuplicateTitleAsync(int journeyId, CancellationToken token);
        Task<bool> HasAnyStepWithAutoTransferMethodAsync(int journeyId, AutoTransferMethod autoTransferMethod, CancellationToken token);
        Task<bool> HasOtherStepWithAutoTransferMethodAsync(int journeyId, int stepId, AutoTransferMethod autoTransferMethod, CancellationToken token);
    }
}
