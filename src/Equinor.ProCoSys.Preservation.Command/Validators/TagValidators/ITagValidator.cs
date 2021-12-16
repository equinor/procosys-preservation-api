using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Command.Validators.TagValidators
{
    public interface ITagValidator
    {
        Task<bool> ExistsAsync(int tagId, CancellationToken token);
        
        Task<bool> ExistsActionAsync(int tagId, int actionId, CancellationToken token);
        
        Task<bool> ExistsRequirementAsync(int tagId, int requirementId, CancellationToken token);
        
        Task<bool> ExistsFieldForRequirementAsync(int tagId, int requirementId, int fieldId, CancellationToken token);

        Task<bool> ExistsTagAttachmentAsync(int tagId, int attachmentId, CancellationToken token);

        Task<bool> ExistsActionAttachmentAsync(int tagId, int actionId, int attachmentId, CancellationToken token);

        Task<bool> ExistsAsync(string tagNo, string projectName, CancellationToken token);

        Task<bool> ExistsAsync(string tagNo, int tagId, CancellationToken token);

        Task<bool> IsVoidedAsync(int tagId, CancellationToken token);
        
        Task<bool> VerifyPreservationStatusAsync(int tagId, PreservationStatus status, CancellationToken token);
        
        Task<bool> VerifyTagTypeAsync(int tagId, TagType tagType, CancellationToken token);
        
        Task<bool> VerifyTagIsAreaTagAsync(int tagId, CancellationToken token);
        
        Task<bool> HasANonVoidedRequirementAsync(int tagId, CancellationToken token);

        Task<bool> IsReadyToBePreservedAsync(int tagId, CancellationToken token);
        
        Task<bool> HasRequirementWithActivePeriodAsync(int tagId, int requirementId, CancellationToken token);
        
        Task<bool> RequirementIsReadyToBePreservedAsync(int tagId, int requirementId, CancellationToken token);
        
        Task<bool> AttachmentWithFilenameExistsAsync(int tagId, string fileName, CancellationToken token);

        Task<bool> IsReadyToBeStartedAsync(int tagId, CancellationToken token);

        Task<bool> IsReadyToUndoStartedAsync(int tagId, CancellationToken token);

        Task<bool> IsReadyToBeCompletedAsync(int tagId, CancellationToken token);
        
        Task<bool> IsReadyToBeDuplicatedAsync(int tagId, CancellationToken token);
            
        Task<bool> IsReadyToBeTransferredAsync(int tagId, CancellationToken token);
        
        Task<bool> IsReadyToBeRescheduledAsync(int tagId, CancellationToken token);

        Task<bool> HasRequirementAsync(int tagId, int tagRequirementId, CancellationToken token);
        
        Task<bool> AllRequirementsWillBeUniqueAsync(int tagId, List<int> requirementDefinitionIdsToBeAdded, CancellationToken token);

        Task<bool> RequirementUsageWillCoverForSuppliersAsync(
            int tagId,
            List<int> tagRequirementIdsToBeUnvoided,
            List<int> tagRequirementIdsToBeVoided,
            List<int> requirementDefinitionIdsToBeAdded,
            CancellationToken token);

        Task<bool> RequirementUsageCoversForSuppliersAsync(int tagId, CancellationToken token);

        Task<bool> RequirementUsageWillCoverBothForSupplierAndOtherAsync(
            int tagId,
            List<int> tagRequirementIdsToBeUnvoided,
            List<int> tagRequirementIdsToBeVoided,
            List<int> requirementDefinitionIdsToBeAdded,
            CancellationToken token);

        Task<bool> RequirementUsageCoversBothForSupplierAndOtherAsync(int tagId, CancellationToken token);

        Task<bool> RequirementUsageWillCoverForOtherThanSuppliersAsync(
            int tagId,
            List<int> tagRequirementIdsToBeUnvoided,
            List<int> tagRequirementIdsToBeVoided,
            List<int> requirementDefinitionIdsToBeAdded,
            CancellationToken token);

        Task<bool> RequirementUsageCoversForOtherThanSuppliersAsync(int tagId, CancellationToken token);

        Task<bool> RequirementWillGetAnyForOtherThanSuppliersUsageAsync(
            int tagId,
            List<int> tagRequirementIdsToBeUnvoided,
            List<int> tagRequirementIdsToBeVoided,
            List<int> requirementDefinitionIdsToBeAdded,
            CancellationToken token);

        Task<bool> RequirementHasAnyForOtherThanSuppliersUsageAsync(int tagId, CancellationToken token);

        Task<bool> IsInUseAsync(long tagId, CancellationToken token);
        
        Task<bool> HasStepAsync(int tagId, int stepId, CancellationToken token);

        Task<bool> VerifyTagDescriptionAsync(int tagId, string description, CancellationToken token);

        Task<bool> IsRequirementVoidedAsync(int tagId, int requirementId, CancellationToken token);
        
        Task<bool> HasRequirementCoverageInNextStepAsync(int tagId, CancellationToken token);
    }
}
