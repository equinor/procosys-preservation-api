using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.Procosys.Preservation.Command.Validators.TagValidators
{
    public class TagValidator : ITagValidator
    {
        private readonly IReadOnlyContext _context;

        public TagValidator(IReadOnlyContext context) => _context = context;

        public async Task<bool> ExistsAsync(int tagId, CancellationToken cancellationToken) =>
            await (from t in _context.QuerySet<Tag>()
                where t.Id == tagId
                select t).AnyAsync(cancellationToken);

        public async Task<bool> ExistsAsync(string tagNo, string projectName, CancellationToken cancellationToken) =>
            await (from tag in _context.QuerySet<Tag>()
                join p in _context.QuerySet<Project>() on EF.Property<int>(tag, "ProjectId") equals p.Id
                where tag.TagNo == tagNo && p.Name == projectName
                select p).AnyAsync(cancellationToken);

        public async Task<bool> IsVoidedAsync(int tagId, CancellationToken cancellationToken)
        {
            var tag = await GetTagWithoutIncludes(tagId, cancellationToken);
            return tag != null && tag.IsVoided;
        }

        public async Task<bool> VerifyPreservationStatusAsync(int tagId, PreservationStatus status, CancellationToken cancellationToken)
        {
            var tag = await GetTagWithoutIncludes(tagId, cancellationToken);
            return tag != null && tag.Status == status;
        }

        public async Task<bool> HasANonVoidedRequirementAsync(int tagId, CancellationToken cancellationToken)
        {
            var tag = await GetTagWithRequirements(tagId, cancellationToken);
            return tag != null && tag.Requirements.Any(r => !r.IsVoided);
        }

        // todo remove after codereview .. diff get crazy when removing it
        public async Task<bool> AllRequirementDefinitionsExistAsync(int tagId, CancellationToken cancellationToken)
        {
            var tag = await GetTagWithRequirements(tagId, cancellationToken);
            if (tag == null)
            {
                return true;
            }

            var reqDefIds = tag.Requirements.Select(r => r.RequirementDefinitionId).Distinct().ToList();
            var reqDefCount = await (from rd in _context.QuerySet<Domain.AggregateModels.RequirementTypeAggregate.RequirementDefinition>()
                                     where reqDefIds.Contains(rd.Id)
                                     select rd)
                .CountAsync(cancellationToken);

            return reqDefCount == reqDefIds.Count;
        }

        public async Task<bool> ReadyToBePreservedAsync(int tagId, CancellationToken cancellationToken)
        {
            var tag = await GetTagWithPreservationPeriods(tagId, cancellationToken);
            if (tag == null)
            {
                return false;
            }

            return tag.IsReadyToBePreserved();
        }

        public async Task<bool> HasRequirementWithActivePeriodAsync(int tagId, int requirementId, CancellationToken cancellationToken)
        {
            var tag = await GetTagWithPreservationPeriods(tagId, cancellationToken);
            if (tag == null)
            {
                return false;
            }

            var requirement = tag.Requirements.SingleOrDefault(r => r.Id == requirementId);

            return requirement != null && requirement.HasActivePeriod;
        }

        // todo remove after codereview .. diff get crazy when removing it
        public async Task<bool> HaveNextStepAsync(int tagId, CancellationToken cancellationToken)
        {
            var tag = await GetTagWithoutIncludes(tagId, cancellationToken);
            if (tag == null)
            {
                return false;
            }

            var journey = await (from j in _context.QuerySet<Domain.AggregateModels.JourneyAggregate.Journey>().Include(j => j.Steps)
                                 where j.Steps.Any(s => s.Id == tag.StepId)
                                 select j).SingleOrDefaultAsync(cancellationToken);

            var step = journey?.GetNextStep(tag.StepId);

            return step != null;
        }

        public async Task<bool> RequirementIsReadyToBePreservedAsync(int tagId, int requirementId, CancellationToken cancellationToken)
        {
            var tag = await GetTagWithPreservationPeriods(tagId, cancellationToken);
            if (tag == null)
            {
                return false;
            }

            var requirement = tag.Requirements.SingleOrDefault(r => r.Id == requirementId);

            return requirement != null && requirement.ReadyToBePreserved;
        }

        // todo remove after codereview .. diff get crazy when removing it
        public async Task<bool> TagFollowsAJourneyAsync(int tagId, CancellationToken cancellationToken)
        {
            var tag = await GetTagWithoutIncludes(tagId, cancellationToken);
            if (tag == null)
            {
                return false;
            }

            return tag.TagType == TagType.PreArea || tag.TagType == TagType.Standard;
        }

        public async Task<bool> IsReadyToBeStartedAsync(int tagId, CancellationToken cancellationToken)
        {
            var tag = await GetTagWithRequirements(tagId, cancellationToken);
            if (tag == null)
            {
                return false;
            }

            return tag.IsReadyToBeStarted();
        }

        public async Task<bool> IsReadyToBeStoppedAsync(int tagId, CancellationToken cancellationToken)
        {
            var tag = await GetTagWithoutIncludes(tagId, cancellationToken);
            if (tag == null)
            {
                return false;
            }
                        
            var journey = await (from j in _context.QuerySet<Domain.AggregateModels.JourneyAggregate.Journey>().Include(j => j.Steps)
                where j.Steps.Any(s => s.Id == tag.StepId)
                select j).SingleOrDefaultAsync(cancellationToken);

            return tag.IsReadyToBeStopped(journey);
        }

        public async Task<bool> IsReadyToBeTransferredAsync(int tagId, CancellationToken cancellationToken)
        {
            var tag = await GetTagWithoutIncludes(tagId, cancellationToken);
            if (tag == null)
            {
                return false;
            }
                        
            var journey = await (from j in _context.QuerySet<Domain.AggregateModels.JourneyAggregate.Journey>().Include(j => j.Steps)
                where j.Steps.Any(s => s.Id == tag.StepId)
                select j).SingleOrDefaultAsync(cancellationToken);

            return tag.IsReadyToBeTransferred(journey);
        }

        private async Task<Tag> GetTagWithoutIncludes(int tagId, CancellationToken cancellationToken)
        {
            var tag = await (from t in _context.QuerySet<Tag>()
                where t.Id == tagId
                select t).SingleOrDefaultAsync(cancellationToken);
            return tag;
        }

        private async Task<Tag> GetTagWithRequirements(int tagId, CancellationToken cancellationToken)
        {
            var tag = await (from t in _context.QuerySet<Tag>().Include(t => t.Requirements)
                where t.Id == tagId
                select t).SingleOrDefaultAsync(cancellationToken);
            return tag;
        }

        private async Task<Tag> GetTagWithPreservationPeriods(int tagId, CancellationToken cancellationToken)
        {
            var tag = await (from t in _context.QuerySet<Tag>().Include(t => t.Requirements)
                    .ThenInclude(r => r.PreservationPeriods)
                where t.Id == tagId
                select t).SingleOrDefaultAsync(cancellationToken);
            return tag;
        }
    }
}
