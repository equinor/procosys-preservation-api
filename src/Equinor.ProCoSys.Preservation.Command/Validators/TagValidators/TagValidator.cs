using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Validators.RequirementDefinitionValidators;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.ProCoSys.Preservation.Command.Validators.TagValidators
{
    public class TagValidator : ITagValidator
    {
        private readonly IReadOnlyContext _context;
        private readonly IRequirementDefinitionValidator _requirementDefinitionValidator;

        public TagValidator(IReadOnlyContext context, IRequirementDefinitionValidator requirementDefinitionValidator)
        {
            _context = context;
            _requirementDefinitionValidator = requirementDefinitionValidator;
        }

        public async Task<bool> ExistsAsync(int tagId, CancellationToken token) =>
            await (from t in _context.QuerySet<Tag>()
                where t.Id == tagId
                select t).AnyAsync(token);

        public async Task<bool> ExistsRequirementAsync(int tagId, int requirementId, CancellationToken token) =>
            await (from t in _context.QuerySet<Tag>()
                join tr in _context.QuerySet<TagRequirement>() on t.Id equals EF.Property<int>(tr, "TagId")
                where t.Id == tagId && tr.Id == requirementId
                select tr).AnyAsync(token);

        public async Task<bool> ExistsFieldForRequirementAsync(int tagId, int requirementId, int fieldId, CancellationToken token)
        {
            var tagRequirement = await (from t in _context.QuerySet<Tag>()
                join tr in _context.QuerySet<TagRequirement>() on t.Id equals EF.Property<int>(tr, "TagId")
                where t.Id == tagId && tr.Id == requirementId
                select tr).SingleOrDefaultAsync(token);

            if (tagRequirement == null)
            {
                return false;
            }

            return await _requirementDefinitionValidator.ExistsFieldAsync(tagRequirement.RequirementDefinitionId, fieldId, token);
        }

        public async Task<bool> ExistsActionAsync(int tagId, int actionId, CancellationToken token) =>
            await (from t in _context.QuerySet<Tag>()
                join action in _context.QuerySet<Action>() on t.Id equals EF.Property<int>(action, "TagId")
                where t.Id == tagId && action.Id == actionId
                select action).AnyAsync(token);

        public async Task<bool> ExistsTagAttachmentAsync(int tagId, int attachmentId, CancellationToken token) =>
            await (from t in _context.QuerySet<Tag>()
                join att in _context.QuerySet<TagAttachment>() on t.Id equals EF.Property<int>(att, "TagId")
                where t.Id == tagId && att.Id == attachmentId
                select att).AnyAsync(token);

        public async Task<bool> ExistsActionAttachmentAsync(int tagId, int actionId, int attachmentId, CancellationToken token) =>
            await (from t in _context.QuerySet<Tag>()
                join action in _context.QuerySet<Action>() on t.Id equals EF.Property<int>(action, "TagId")
                join att in _context.QuerySet<ActionAttachment>() on action.Id equals EF.Property<int>(att, "ActionId")
                where t.Id == tagId && action.Id == actionId && att.Id == attachmentId
                select att).AnyAsync(token);

        public async Task<bool> ExistsAsync(string tagNo, string projectName, CancellationToken token) =>
            await (from tag in _context.QuerySet<Tag>()
                   join p in _context.QuerySet<Project>() on EF.Property<int>(tag, "ProjectId") equals p.Id
                   where tag.TagNo == tagNo && p.Name == projectName
                   select p).AnyAsync(token);

        public async Task<bool> ExistsAsync(string tagNo, int tagId, CancellationToken token)
        {
            var project = await (from p in _context.QuerySet<Project>()
                           join tag in _context.QuerySet<Tag>() on p.Id equals EF.Property<int>(tag, "ProjectId")
                           where tag.Id == tagId
                           select p).SingleOrDefaultAsync(token);
            if (project == null)
            {
                return false;
            }
            return await ExistsAsync(tagNo, project.Name, token);
        }

        public async Task<bool> IsVoidedAsync(int tagId, CancellationToken token)
        {
            var tag = await GetTagWithoutIncludesAsync(tagId, token);
            return tag != null && tag.IsVoided;
        }

        public async Task<bool> IsVoidedInSourceAsync(int tagId, CancellationToken token)
        {
            var tag = await GetTagWithoutIncludesAsync(tagId, token);
            return tag != null && tag.IsVoidedInSource;
        }

        public async Task<bool> VerifyPreservationStatusAsync(int tagId, PreservationStatus status, CancellationToken token)
        {
            var tag = await GetTagWithoutIncludesAsync(tagId, token);
            return tag != null && tag.Status == status;
        }

        public async Task<bool> VerifyTagTypeAsync(int tagId, TagType tagType, CancellationToken token)
        {
            var tag = await GetTagWithoutIncludesAsync(tagId, token);
            return tag != null && tag.TagType == tagType;
        }

        public async Task<bool> VerifyTagIsAreaTagAsync(int tagId, CancellationToken token)
        {
            var tag = await GetTagWithoutIncludesAsync(tagId, token);
            return tag != null && tag.IsAreaTag();
        }

        public async Task<bool> HasANonVoidedRequirementAsync(int tagId, CancellationToken token)
        {
            var tag = await GetTagWithRequirementsAsync(tagId, token);
            return tag != null && tag.Requirements.Any(r => !r.IsVoided);
        }

        public async Task<bool> IsReadyToBePreservedAsync(int tagId, CancellationToken token)
        {
            var tag = await GetTagWithPreservationPeriodsAsync(tagId, token);
            if (tag == null)
            {
                return false;
            }

            return tag.IsReadyToBePreserved();
        }

        public async Task<bool> HasRequirementWithActivePeriodAsync(int tagId, int requirementId, CancellationToken token)
        {
            var tag = await GetTagWithPreservationPeriodsAsync(tagId, token);
            if (tag == null)
            {
                return false;
            }

            var requirement = tag.Requirements.SingleOrDefault(r => r.Id == requirementId);

            return requirement != null && requirement.HasActivePeriod;
        }

        public async Task<bool> RequirementIsReadyToBePreservedAsync(int tagId, int requirementId, CancellationToken token)
        {
            var tag = await GetTagWithPreservationPeriodsAsync(tagId, token);
            if (tag == null)
            {
                return false;
            }

            var requirement = tag.Requirements.SingleOrDefault(r => r.Id == requirementId);

            return requirement != null && requirement.ReadyToBePreserved;
        }

        public async Task<bool> IsReadyToBeStartedAsync(int tagId, CancellationToken token)
        {
            var tag = await GetTagWithRequirementsAsync(tagId, token);
            if (tag == null)
            {
                return false;
            }

            return tag.IsReadyToBeStarted();
        }

        public async Task<bool> IsReadyToUndoStartedAsync(int tagId, CancellationToken token)
        {
            var tag = await GetTagWithoutIncludesAsync(tagId, token);
            if (tag == null)
            {
                return false;
            }

            return tag.IsReadyToUndoStarted();
        }

        public async Task<bool> IsReadyToBeCompletedAsync(int tagId, CancellationToken token)
        {
            var tag = await GetTagWithoutIncludesAsync(tagId, token);
            if (tag == null)
            {
                return false;
            }
                        
            var journey = await (from j in _context.QuerySet<Journey>().Include(j => j.Steps)
                where j.Steps.Any(s => s.Id == tag.StepId)
                select j).SingleOrDefaultAsync(token);

            return tag.IsReadyToBeCompleted(journey);
        }

        public async Task<bool> IsReadyToBeDuplicatedAsync(int tagId, CancellationToken token)
        {
            var tag = await GetTagWithoutIncludesAsync(tagId, token);
            if (tag == null)
            {
                return false;
            }
                        
            return tag.IsReadyToBeDuplicated();
        }

        public async Task<bool> IsReadyToBeTransferredAsync(int tagId, CancellationToken token)
        {
            var tag = await GetTagWithoutIncludesAsync(tagId, token);
            if (tag == null)
            {
                return false;
            }
                        
            var journey = await (from j in _context.QuerySet<Journey>().Include(j => j.Steps)
                where j.Steps.Any(s => s.Id == tag.StepId)
                select j).SingleOrDefaultAsync(token);

            return tag.IsReadyToBeTransferred(journey);
        }
        
        public async Task<bool> IsReadyToBeRescheduledAsync(int tagId, CancellationToken token)
        {
            var tag = await GetTagWithoutIncludesAsync(tagId, token);
            if (tag == null)
            {
                return false;
            }

            return tag.IsReadyToBeRescheduled();
        }

        public async Task<bool> IsReadyToBeEditedAsync(int tagId, CancellationToken token)
        {
            var tag = await GetTagWithoutIncludesAsync(tagId, token);
            if (tag == null)
            {
                return false;
            }

            return tag.IsReadyToBeEdited();
        }

        public async Task<bool> AttachmentWithFilenameExistsAsync(int tagId, string fileName, CancellationToken token)
        {
            var tag = await GetTagWithAttachmentsAsync(tagId, token);

            return tag?.GetAttachmentByFileName(fileName) != null;
        }

        public async Task<bool> HasRequirementAsync(int tagId, int tagRequirementId, CancellationToken token)
        {
            var tag = await GetTagWithRequirementsAsync(tagId, token);
            if (tag == null)
            {
                return false;
            }

            var requirement = tag.Requirements.SingleOrDefault(r => r.Id == tagRequirementId);

            return requirement != null;
        }

        public async Task<bool> AllRequirementsWillBeUniqueAsync(int tagId, List<int> requirementDefinitionIdsToBeAdded, CancellationToken token)
        {
            var tag = await GetTagWithRequirementsAsync(tagId, token);

            if (tag == null)
            {
                return false;
            }
            if (requirementDefinitionIdsToBeAdded.Count == 0)
            {
                return true;
            }

            var allRequirementDefinitionIds = tag.Requirements.Select(r => r.RequirementDefinitionId).ToList();
            allRequirementDefinitionIds.AddRange(requirementDefinitionIdsToBeAdded);
            return allRequirementDefinitionIds.Count == allRequirementDefinitionIds.Distinct().Count();
        }

        public async Task<bool> RequirementUsageWillCoverForSuppliersAsync(
            int tagId,
            List<int> tagRequirementIdsToBeUnvoided,
            List<int> tagRequirementIdsToBeVoided,
            List<int> requirementDefinitionIdsToBeAdded,
            CancellationToken token)
        {
            var (tag, requirementDefinitionIds) = await GetNonVoidedRequirementDefinitionIdsAsync(
                tagId,
                tagRequirementIdsToBeUnvoided,
                tagRequirementIdsToBeVoided,
                requirementDefinitionIdsToBeAdded,
                token);

            if (tag == null)
            {
                return false;
            }

            return await _requirementDefinitionValidator.UsageCoversForSuppliersAsync(requirementDefinitionIds, token);
        }

        public async Task<bool> RequirementUsageCoversForSuppliersAsync(int tagId, CancellationToken token)
        {
            var (tag, requirementDefinitionIds) = await GetNonVoidedRequirementDefinitionIdsAsync(tagId, token);

            if (tag == null)
            {
                return false;
            }

            return await _requirementDefinitionValidator.UsageCoversForSuppliersAsync(requirementDefinitionIds, token);
        }

        public async Task<bool> RequirementUsageWillCoverBothForSupplierAndOtherAsync(
            int tagId,
            List<int> tagRequirementIdsToBeUnvoided,
            List<int> tagRequirementIdsToBeVoided,
            List<int> requirementDefinitionIdsToBeAdded,
            CancellationToken token)
        {
            var (tag, requirementDefinitionIds) = await GetNonVoidedRequirementDefinitionIdsAsync(
                tagId,
                tagRequirementIdsToBeUnvoided,
                tagRequirementIdsToBeVoided,
                requirementDefinitionIdsToBeAdded,
                token);

            if (tag == null)
            {
                return false;
            }

            return await _requirementDefinitionValidator.UsageCoversBothForSupplierAndOtherAsync(requirementDefinitionIds, token);
        }

        public async Task<bool> RequirementUsageCoversBothForSupplierAndOtherAsync(int tagId, CancellationToken token)
        {
            var (tag, requirementDefinitionIds) = await GetNonVoidedRequirementDefinitionIdsAsync(tagId, token);

            if (tag == null)
            {
                return false;
            }

            return await _requirementDefinitionValidator.UsageCoversBothForSupplierAndOtherAsync(requirementDefinitionIds, token);
        }

        public async Task<bool> RequirementUsageWillCoverForOtherThanSuppliersAsync(
            int tagId,
            List<int> tagRequirementIdsToBeUnvoided,
            List<int> tagRequirementIdsToBeVoided,
            List<int> requirementDefinitionIdsToBeAdded,
            CancellationToken token)
        {
            var (tag, requirementDefinitionIds) = await GetNonVoidedRequirementDefinitionIdsAsync(
                tagId,
                tagRequirementIdsToBeUnvoided,
                tagRequirementIdsToBeVoided,
                requirementDefinitionIdsToBeAdded,
                token);

            if (tag == null)
            {
                return false;
            }

            return await _requirementDefinitionValidator.UsageCoversForOtherThanSuppliersAsync(requirementDefinitionIds, token);
        }

        public async Task<bool> RequirementUsageCoversForOtherThanSuppliersAsync(int tagId, CancellationToken token)
        {
            var (tag, requirementDefinitionIds) = await GetNonVoidedRequirementDefinitionIdsAsync(tagId, token);

            if (tag == null)
            {
                return false;
            }

            return await _requirementDefinitionValidator.UsageCoversForOtherThanSuppliersAsync(requirementDefinitionIds, token);
        }

        public async Task<bool> RequirementWillGetAnyForOtherThanSuppliersUsageAsync(int tagId,
            List<int> tagRequirementIdsToBeUnvoided,
            List<int> tagRequirementIdsToBeVoided,
            List<int> requirementDefinitionIdsToBeAdded,
            CancellationToken token)
        {
            var (tag, requirementDefinitionIds) = await GetNonVoidedRequirementDefinitionIdsAsync(
                tagId,
                tagRequirementIdsToBeUnvoided,
                tagRequirementIdsToBeVoided,
                requirementDefinitionIdsToBeAdded,
                token);

            if (tag == null)
            {
                return false;
            }

            return await _requirementDefinitionValidator.HasAnyForForOtherThanSuppliersUsageAsync(requirementDefinitionIds, token);
        }

        public async Task<bool> RequirementHasAnyForOtherThanSuppliersUsageAsync(int tagId, CancellationToken token)
        {
            var (tag, requirementDefinitionIds) = await GetNonVoidedRequirementDefinitionIdsAsync(tagId, token);

            if (tag == null)
            {
                return false;
            }

            return await _requirementDefinitionValidator.HasAnyForForOtherThanSuppliersUsageAsync(requirementDefinitionIds, token);
        }

        public async Task<bool> VerifyTagDescriptionAsync(int tagId, string description, CancellationToken token)
        {
            var tag = await GetTagWithoutIncludesAsync(tagId, token);
            return tag != null && tag.Description == description;
        }

        public async Task<bool> IsRequirementVoidedAsync(int tagId, int requirementId, CancellationToken token)
        {
            var tag = await GetTagWithRequirementsAsync(tagId, token);
            if (tag == null)
            {
                return false;
            }

            var requirement = tag.Requirements.SingleOrDefault(r => r.Id == requirementId);

            return requirement != null && requirement.IsVoided;
        }

        public async Task<bool> HasRequirementCoverageInNextStepAsync(int tagId, CancellationToken token)
        {
            var tag = await GetTagWithRequirementsAsync(tagId, token);
            if (tag == null)
            {
                return false;
            }

            var nonVoidedTagRequirementIds =
                tag.Requirements
                    .Where(r => !r.IsVoided)
                    .Select(r => r.RequirementDefinitionId).ToList();
                                    
            var journey = await (from j in _context.QuerySet<Journey>().Include(j => j.Steps)
                where j.Steps.Any(s => s.Id == tag.StepId)
                select j).SingleOrDefaultAsync(token);

            var nextStep = journey.GetNextStep(tag.StepId);

            if (nextStep == null)
            {
                return false;
            }

            if (nextStep.IsSupplierStep)
            {
                return await _requirementDefinitionValidator.UsageCoversForSuppliersAsync(nonVoidedTagRequirementIds, token);
            }
            
            return await _requirementDefinitionValidator.UsageCoversForOtherThanSuppliersAsync(nonVoidedTagRequirementIds, token);
        }

        public async Task<bool> IsInUseAsync(long tagId, CancellationToken token)
        {
            var inUse = await (from t in _context.QuerySet<Tag>()
                    .Include(t => t.Attachments)
                    .Include(t => t.Actions)
                where t.Id == tagId &&
                      (t.Status != PreservationStatus.NotStarted || t.Attachments.Any() || t.Actions.Any())
                select t.Id).AnyAsync(token);

            return inUse;
        }

        public async Task<bool> HasStepAsync(int tagId, int stepId, CancellationToken token)
        {
            var tag = await GetTagWithoutIncludesAsync(tagId, token);
            return tag != null && tag.StepId == stepId;
        }

        public async Task<bool> IsInASupplierStepAsync(int tagId, CancellationToken token)
        {
            var tag = await GetTagWithoutIncludesAsync(tagId, token);
            if (tag == null)
            {
                return false;
            }

            var step = await (from s in _context.QuerySet<Step>()
                              where s.Id == tag.StepId
                              select s).SingleOrDefaultAsync(token);

            return step != null && step.IsSupplierStep;
        }

        public async Task<string> GetTagDetailsAsync(int tagId, CancellationToken token)
        {
            var tag = await GetTagWithoutIncludesAsync(tagId, token);
            return tag != null ? tag.TagNo : string.Empty;
        }

        private async Task<Tag> GetTagWithoutIncludesAsync(int tagId, CancellationToken token)
        {
            var tag = await (from t in _context.QuerySet<Tag>()
                where t.Id == tagId
                select t).SingleOrDefaultAsync(token);
            return tag;
        }

        private async Task<Tag> GetTagWithRequirementsAsync(int tagId, CancellationToken token)
        {
            var tag = await (from t in _context.QuerySet<Tag>().Include(t => t.Requirements)
                where t.Id == tagId
                select t).SingleOrDefaultAsync(token);
            return tag;
        }

        private async Task<Tag> GetTagWithPreservationPeriodsAsync(int tagId, CancellationToken token)
        {
            var tag = await (from t in _context.QuerySet<Tag>().Include(t => t.Requirements)
                    .ThenInclude(r => r.PreservationPeriods)
                where t.Id == tagId
                select t).SingleOrDefaultAsync(token);
            return tag;
        }

        private async Task<Tag> GetTagWithAttachmentsAsync(int tagId, CancellationToken token)
        {
            var tag = await (from t in _context.QuerySet<Tag>().Include(t => t.Attachments)
                where t.Id == tagId
                select t).SingleOrDefaultAsync(token);
            return tag;
        }

        private async Task<(Tag, List<int>)> GetNonVoidedRequirementDefinitionIdsAsync(
            int tagId,
            List<int> tagRequirementIdsToBeUnvoided,
            List<int> tagRequirementIdsToBeVoided,
            List<int> requirementDefinitionIdsToBeAdded,
            CancellationToken token)
        {
            var tag = await GetTagWithRequirementsAsync(tagId, token);
            if (tag == null)
            {
                return (null, new List<int>());
            }

            var nonVoidedTagRequirementIds = 
                tag.Requirements
                    .Where(r => !r.IsVoided || 
                                (r.IsVoided && tagRequirementIdsToBeUnvoided.Contains(r.Id)))
                    .Select(r => r.Id)
                    .Except(tagRequirementIdsToBeVoided).ToList();
            
            var nonVoidedRequirementDefinitionIds = 
                tag.Requirements
                    .Where(r => nonVoidedTagRequirementIds.Contains(r.Id))
                    .Select(r => r.RequirementDefinitionId).ToList();
            nonVoidedRequirementDefinitionIds.AddRange(requirementDefinitionIdsToBeAdded);

            return (tag, nonVoidedRequirementDefinitionIds);
        }

        private async Task<(Tag, List<int>)> GetNonVoidedRequirementDefinitionIdsAsync(int tagId, CancellationToken token)
        {
            var tag = await GetTagWithRequirementsAsync(tagId, token);
            if (tag == null)
            {
                return (null, new List<int>());
            }

            var nonVoidedTagRequirementIds =
                tag.Requirements
                    .Where(r => !r.IsVoided)
                    .Select(r => r.Id).ToList();

            var nonVoidedRequirementDefinitionIds =
                tag.Requirements
                    .Where(r => nonVoidedTagRequirementIds.Contains(r.Id))
                    .Select(r => r.RequirementDefinitionId).ToList();

            return (tag, nonVoidedRequirementDefinitionIds);
        }
    }
}
