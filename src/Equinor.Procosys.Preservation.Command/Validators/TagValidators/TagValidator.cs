using System;
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

        public async Task<bool> ExistsAsync(int tagId, CancellationToken cancellationToken)
        {
            var count = await (from t in _context.QuerySet<Tag>()
                where t.Id == tagId
                select t).CountAsync(cancellationToken);
            return count > 0;
        }

        public async Task<bool> ExistsAsync(string tagNo, string projectName, CancellationToken cancellationToken)
        {
            var count = await (from tag in _context.QuerySet<Tag>()
                join p in _context.QuerySet<Project>() on EF.Property<int>(tag, "ProjectId") equals p.Id
                where tag.TagNo == tagNo && p.Name == projectName
                select p).CountAsync(cancellationToken);
            return count > 0;
        }

        public async Task<bool> IsVoidedAsync(int tagId, CancellationToken cancellationToken)
        {
            var tag = await (from t in _context.QuerySet<Tag>()
                where t.Id == tagId
                select t).FirstOrDefaultAsync(cancellationToken);
            return tag != null && tag.IsVoided;
        }

        public async Task<bool> VerifyPreservationStatusAsync(int tagId, PreservationStatus status, CancellationToken cancellationToken)
        {
            var tag = await (from t in _context.QuerySet<Tag>()
                where t.Id == tagId
                select t).FirstOrDefaultAsync(cancellationToken);
            return tag != null && tag.Status == status;
        }

        public async Task<bool> HasANonVoidedRequirementAsync(int tagId, CancellationToken cancellationToken)
        {
            var tag = await (from t in _context.QuerySet<Tag>().Include(t => t.Requirements)
                where t.Id == tagId
                select t).FirstOrDefaultAsync(cancellationToken);
            return tag?.Requirements != null && tag.Requirements.Any(r => !r.IsVoided);
        }

        public async Task<bool> AllRequirementDefinitionsExistAsync(int tagId, CancellationToken cancellationToken)
        {
            var tag = await (from t in _context.QuerySet<Tag>().Include(t => t.Requirements)
                where t.Id == tagId
                select t).FirstOrDefaultAsync(cancellationToken);
            if (tag?.Requirements == null)
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

        public async Task<bool> ReadyToBePreservedAsync(int tagId, DateTime preservedAtUtc, CancellationToken cancellationToken)
        {
            var tag = await (from t in _context.QuerySet<Tag>().Include(t => t.Requirements).ThenInclude(r => r.PreservationPeriods)
                where t.Id == tagId
                select t).FirstOrDefaultAsync(cancellationToken);
            if (tag == null)
            {
                return false;
            }

            return tag.IsReadyToBePreserved(preservedAtUtc);
        }

        public async Task<bool> HaveRequirementWithActivePeriodAsync(int tagId, int requirementId, CancellationToken cancellationToken)
        {
            var tag = await (from t in _context.QuerySet<Tag>().Include(t => t.Requirements).ThenInclude(r => r.PreservationPeriods)
                where t.Id == tagId
                select t).FirstOrDefaultAsync(cancellationToken);
            if (tag == null)
            {
                return false;
            }

            var requirement = tag.Requirements.SingleOrDefault(r => r.Id == requirementId);

            return requirement != null && requirement.HasActivePeriod;
        }

        public async Task<bool> HaveNextStepAsync(int tagId, CancellationToken cancellationToken)
        {
            var tag = await (from t in _context.QuerySet<Tag>().Include(t => t.Requirements)
                where t.Id == tagId
                select t).FirstOrDefaultAsync(cancellationToken);
            if (tag == null)
            {
                return false;
            }

            var journey = await (from j in _context.QuerySet<Domain.AggregateModels.JourneyAggregate.Journey>().Include(j => j.Steps)
                where j.Steps.Any(s => s.Id == tag.StepId)
                select j).FirstOrDefaultAsync(cancellationToken);

            var step = journey?.GetNextStep(tag.StepId);

            return step != null;
        }

        public async Task<bool> HasRequirementReadyToBePreservedAsync(int tagId, int requirementId, CancellationToken cancellationToken)
        {
            var tag = await (from t in _context.QuerySet<Tag>().Include(t => t.Requirements).ThenInclude(r => r.PreservationPeriods)
                where t.Id == tagId
                select t).FirstOrDefaultAsync(cancellationToken);
            if (tag == null)
            {
                return false;
            }

            var requirement = tag.Requirements.SingleOrDefault(r => r.Id == requirementId);

            return requirement != null && requirement.ReadyToBePreserved;
        }
    }
}
