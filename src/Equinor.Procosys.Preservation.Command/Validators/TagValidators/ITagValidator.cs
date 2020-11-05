using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.Procosys.Preservation.Command.Validators.TagValidators
{
    public interface ITagValidator
    {
        Task<bool> ExistsAsync(int tagId, CancellationToken token);

        Task<bool> ExistsAsync(string tagNo, string projectName, CancellationToken token);

        Task<bool> ExistsAsync(string tagNo, int tagId, CancellationToken token);

        Task<bool> IsVoidedAsync(int tagId, CancellationToken token);
        
        Task<bool> VerifyPreservationStatusAsync(int tagId, PreservationStatus status, CancellationToken token);
        
        Task<bool> VerifyTagTypeAsync(int tagId, TagType tagType, CancellationToken token);
        
        Task<bool> VerifyTagIsAreaTagAsync(int tagId, CancellationToken token);
        
        Task<bool> HasANonVoidedRequirementAsync(int tagId, CancellationToken token);

        Task<bool> ReadyToBePreservedAsync(int tagId, CancellationToken token);
        
        Task<bool> HasRequirementWithActivePeriodAsync(int tagId, int requirementId, CancellationToken token);
        
        Task<bool> RequirementIsReadyToBePreservedAsync(int tagId, int requirementId, CancellationToken token);
        
        Task<bool> AttachmentWithFilenameExistsAsync(int tagId, string fileName, CancellationToken token);

        Task<bool> IsReadyToBeStartedAsync(int tagId, CancellationToken token);
        
        Task<bool> IsReadyToBeCompletedAsync(int tagId, CancellationToken token);
            
        Task<bool> IsReadyToBeTransferredAsync(int tagId, CancellationToken token);
        
        Task<bool> IsReadyToBeRescheduledAsync(int tagId, CancellationToken token);

        Task<bool> HasRequirementAsync(int tagId, int tagRequirementId, CancellationToken token);
        
        Task<bool> AllRequirementsWillBeUniqueAsync(int tagId, List<int> requirementDefinitionIdsToBeAdded, CancellationToken token);
        
        Task<bool> UsageCoversBothForSupplierAndOtherAsync(int tagId, List<int> tagRequirementIdsToBeVoided, List<int> requirementDefinitionIdsToBeAdded, CancellationToken token);
        
        Task<bool> UsageCoversForOtherThanSuppliersAsync(int tagId, List<int> tagRequirementIdsToBeVoided, List<int> requirementDefinitionIdsToBeAdded, CancellationToken token);

        Task<bool> IsInUseAsync(long tagId, CancellationToken token);
        
        Task<bool> HasStepAsync(int tagId, int stepId, CancellationToken token);
    }
}
