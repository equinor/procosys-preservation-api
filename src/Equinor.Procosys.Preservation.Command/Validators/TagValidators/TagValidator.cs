using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.RequirementDefinitionValidators;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.Procosys.Preservation.Command.Validators.TagValidators
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

        public async Task<bool> ExistsAsync(string tagNo, string projectName, CancellationToken token) =>
            await (from tag in _context.QuerySet<Tag>()
                join p in _context.QuerySet<Project>() on EF.Property<int>(tag, "ProjectId") equals p.Id
                where tag.TagNo == tagNo && p.Name == projectName
                select p).AnyAsync(token);

        public async Task<bool> IsVoidedAsync(int tagId, CancellationToken token)
        {
            var tag = await GetTagWithoutIncludes(tagId, token);
            return tag != null && tag.IsVoided;
        }

        public async Task<bool> VerifyPreservationStatusAsync(int tagId, PreservationStatus status, CancellationToken token)
        {
            var tag = await GetTagWithoutIncludes(tagId, token);
            return tag != null && tag.Status == status;
        }

        public async Task<bool> HasANonVoidedRequirementAsync(int tagId, CancellationToken token)
        {
            var tag = await GetTagWithRequirements(tagId, token);
            return tag != null && tag.Requirements.Any(r => !r.IsVoided);
        }

        public async Task<bool> ReadyToBePreservedAsync(int tagId, CancellationToken token)
        {
            var tag = await GetTagWithPreservationPeriods(tagId, token);
            if (tag == null)
            {
                return false;
            }

            return tag.IsReadyToBePreserved();
        }

        public async Task<bool> HasRequirementWithActivePeriodAsync(int tagId, int requirementId, CancellationToken token)
        {
            var tag = await GetTagWithPreservationPeriods(tagId, token);
            if (tag == null)
            {
                return false;
            }

            var requirement = tag.Requirements.SingleOrDefault(r => r.Id == requirementId);

            return requirement != null && requirement.HasActivePeriod;
        }

        public async Task<bool> RequirementIsReadyToBePreservedAsync(int tagId, int requirementId, CancellationToken token)
        {
            var tag = await GetTagWithPreservationPeriods(tagId, token);
            if (tag == null)
            {
                return false;
            }

            var requirement = tag.Requirements.SingleOrDefault(r => r.Id == requirementId);

            return requirement != null && requirement.ReadyToBePreserved;
        }

        public async Task<bool> IsReadyToBeStartedAsync(int tagId, CancellationToken token)
        {
            var tag = await GetTagWithRequirements(tagId, token);
            if (tag == null)
            {
                return false;
            }

            return tag.IsReadyToBeStarted();
        }

        public async Task<bool> IsReadyToBeCompletedAsync(int tagId, CancellationToken token)
        {
            var tag = await GetTagWithoutIncludes(tagId, token);
            if (tag == null)
            {
                return false;
            }
                        
            var journey = await (from j in _context.QuerySet<Journey>().Include(j => j.Steps)
                where j.Steps.Any(s => s.Id == tag.StepId)
                select j).SingleOrDefaultAsync(token);

            return tag.IsReadyToBeCompleted(journey);
        }

        public async Task<bool> IsReadyToBeTransferredAsync(int tagId, CancellationToken token)
        {
            var tag = await GetTagWithoutIncludes(tagId, token);
            if (tag == null)
            {
                return false;
            }
                        
            var journey = await (from j in _context.QuerySet<Journey>().Include(j => j.Steps)
                where j.Steps.Any(s => s.Id == tag.StepId)
                select j).SingleOrDefaultAsync(token);

            return tag.IsReadyToBeTransferred(journey);
        }

        public async Task<bool> AttachmentWithFilenameExistsAsync(int tagId, string fileName, CancellationToken token)
        {
            var tag = await GetTagWithAttachments(tagId, token);

            return tag?.GetAttachmentByFileName(fileName) != null;
        }

        public async Task<bool> HasRequirementAsync(int tagId, int tagRequirementId, CancellationToken token)
        {
            var tag = await GetTagWithRequirements(tagId, token);
            if (tag == null)
            {
                return false;
            }

            var requirement = tag.Requirements.SingleOrDefault(r => r.Id == tagRequirementId);

            return requirement != null;
        }

        public async Task<bool> AllRequirementsWillBeUniqueAsync(int tagId, List<int> requirementDefinitionIdsToBeAdded, CancellationToken token)
        {
            var tag = await GetTagWithRequirements(tagId, token);

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

        public async Task<bool> UsageCoversBothForSupplierAndOtherAsync(
            int tagId,
            List<int> tagRequirementIdsToBeVoided,
            List<int> requirementDefinitionIdsToBeAdded,
            CancellationToken token)
        {
            List<int> requirementDefinitionIds;
            Tag tag;
            (tag, requirementDefinitionIds) = await GetRequirementDefinitionIdsFromNonVoidedRequirements(
                tagId,
                tagRequirementIdsToBeVoided,
                requirementDefinitionIdsToBeAdded,
                token);

            if (tag == null)
            {
                return false;
            }

            return await _requirementDefinitionValidator.UsageCoversBothForSupplierAndOtherAsync(requirementDefinitionIds, token);
        }

        public async Task<bool> UsageCoversForOtherThanSuppliersAsync(
            int tagId,
            List<int> tagRequirementIdsToBeVoided,
            List<int> requirementDefinitionIdsToBeAdded,
            CancellationToken token)
        {
            List<int> requirementDefinitionIds;
            Tag tag;
            (tag, requirementDefinitionIds) = await GetRequirementDefinitionIdsFromNonVoidedRequirements(
                tagId,
                tagRequirementIdsToBeVoided,
                requirementDefinitionIdsToBeAdded,
                token);

            if (tag == null)
            {
                return false;
            }

            return await _requirementDefinitionValidator.UsageCoversForOtherThanSuppliersAsync(requirementDefinitionIds, token);
        }

        public async Task<bool> HasAnyForSupplierOnlyUsageAsync(
            int tagId,
            List<int> tagRequirementIdsToBeVoided,
            List<int> requirementDefinitionIdsToBeAdded,
            CancellationToken token)
        {
            List<int> requirementDefinitionIds;
            Tag tag;
            (tag, requirementDefinitionIds) = await GetRequirementDefinitionIdsFromNonVoidedRequirements(
                tagId,
                tagRequirementIdsToBeVoided,
                requirementDefinitionIdsToBeAdded,
                token);

            if (tag == null)
            {
                return false;
            }

            return await _requirementDefinitionValidator.HasAnyForSupplierOnlyUsageAsync(requirementDefinitionIds, token);
        }

        private async Task<Tag> GetTagWithoutIncludes(int tagId, CancellationToken token)
        {
            var tag = await (from t in _context.QuerySet<Tag>()
                where t.Id == tagId
                select t).SingleOrDefaultAsync(token);
            return tag;
        }

        private async Task<Tag> GetTagWithRequirements(int tagId, CancellationToken token)
        {
            var tag = await (from t in _context.QuerySet<Tag>().Include(t => t.Requirements)
                where t.Id == tagId
                select t).SingleOrDefaultAsync(token);
            return tag;
        }

        private async Task<Tag> GetTagWithPreservationPeriods(int tagId, CancellationToken token)
        {
            var tag = await (from t in _context.QuerySet<Tag>().Include(t => t.Requirements)
                    .ThenInclude(r => r.PreservationPeriods)
                where t.Id == tagId
                select t).SingleOrDefaultAsync(token);
            return tag;
        }

        private async Task<Tag> GetTagWithAttachments(int tagId, CancellationToken token)
        {
            var tag = await (from t in _context.QuerySet<Tag>().Include(t => t.Attachments)
                where t.Id == tagId
                select t).SingleOrDefaultAsync(token);
            return tag;
        }

        private async Task<(Tag, List<int>)> GetRequirementDefinitionIdsFromNonVoidedRequirements(
            int tagId,
            List<int> tagRequirementIdsToBeVoided,
            List<int> requirementDefinitionIdsToBeAdded,
            CancellationToken token)
        {
            var tag = await GetTagWithRequirements(tagId, token);
            if (tag == null)
            {
                return (null, new List<int>());
            }

            var nonVoidedTagRequirementIds = 
                tag.Requirements
                    .Where(r => !r.IsVoided)
                    .Select(r => r.Id)
                    .Except(tagRequirementIdsToBeVoided).ToList();
            
            var nonVoidedRequirementDefinitionIds = 
                tag.Requirements
                    .Where(r => nonVoidedTagRequirementIds.Contains(r.Id))
                    .Select(r => r.RequirementDefinitionId).ToList();
            nonVoidedRequirementDefinitionIds.AddRange(requirementDefinitionIdsToBeAdded);

            return (tag, nonVoidedRequirementDefinitionIds);
        }

        public async Task<bool> IsInUseAsync(long tagId, CancellationToken token)
        {
            var tag = await (from t in _context.QuerySet<Tag>()
                    .Include(t => t.Attachments)
                             where (t.Id == tagId)
                select t).SingleOrDefaultAsync();

            var inUse = await (from t in _context.QuerySet<Tag>()
                    .Include(t => t.Attachments)
                    .Include(t => t.Actions)
                where (t.Id == tagId) && t.Status != PreservationStatus.NotStarted ||
                      t.Attachments.Any() || t.Actions.Any()
                select t.Id).AnyAsync(token);

            return inUse;
        }
    }
}
