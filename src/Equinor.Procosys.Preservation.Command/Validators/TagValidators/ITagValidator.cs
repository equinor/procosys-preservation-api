using System;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.Procosys.Preservation.Command.Validators.TagValidators
{
    public interface ITagValidator
    {
        Task<bool> ExistsAsync(int tagId, CancellationToken token);
        
        Task<bool> ExistsAsync(string tagNo, string projectName, CancellationToken token);

        Task<bool> IsVoidedAsync(int tagId, CancellationToken token);
        
        Task<bool> VerifyPreservationStatusAsync(int tagId, PreservationStatus status, CancellationToken token);
        
        Task<bool> HasANonVoidedRequirementAsync(int tagId, CancellationToken token);
        
        Task<bool> AllRequirementDefinitionsExistAsync(int tagId, CancellationToken token);

        Task<bool> ReadyToBePreservedAsync(int tagId, DateTime preservedAtUtc, CancellationToken token);
        
        Task<bool> HaveRequirementReadyForRecordingAsync(int tagId, int requirementId, CancellationToken token);
        
        Task<bool> HaveNextStepAsync(int tagId, CancellationToken token);
        
        Task<bool> HaveRequirementReadyToBePreservedAsync(int tagId, int requirementId, CancellationToken token);
    }
}
