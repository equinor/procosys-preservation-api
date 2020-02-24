using System;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.Procosys.Preservation.Command.Validators.TagValidators
{
    public interface ITagValidator
    {
        Task<bool> ExistsAsync(int tagId, CancellationToken cancellationToken);
        
        Task<bool> ExistsAsync(string tagNo, string projectName, CancellationToken cancellationToken);

        Task<bool> IsVoidedAsync(int tagId, CancellationToken cancellationToken);
        
        Task<bool> VerifyPreservationStatusAsync(int tagId, PreservationStatus status, CancellationToken cancellationToken);
        
        Task<bool> HasANonVoidedRequirementAsync(int tagId, CancellationToken cancellationToken);
        
        Task<bool> AllRequirementDefinitionsExistAsync(int tagId, CancellationToken cancellationToken);

        Task<bool> ReadyToBePreservedAsync(int tagId, DateTime preservedAtUtc, CancellationToken cancellationToken);
        
        Task<bool> HaveRequirementReadyForRecordingAsync(int tagId, int requirementId, CancellationToken cancellationToken);
        
        Task<bool> HaveNextStepAsync(int tagId, CancellationToken cancellationToken);
        
        Task<bool> HaveRequirementReadyToBePreservedAsync(int tagId, int requirementId, CancellationToken cancellationToken);
    }
}
