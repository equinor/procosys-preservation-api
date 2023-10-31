using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.ProCoSys.Preservation.Infrastructure.Repositories
{
    public class ProjectRepository : RepositoryBase<Project>, IProjectRepository
    {
        public ProjectRepository(PreservationContext context)
            : base(context, context.Projects,
                context.Projects
                    // include preservation history only in DefaultQuery. Don't add other includes as Actions/Attachments due to performance
                    .Include(p => p.Tags)
                        .ThenInclude(t => t.Requirements)
                        .ThenInclude(r => r.PreservationPeriods)
                        .ThenInclude(pp => pp.FieldValues)
                        .ThenInclude(fv => fv.FieldValueAttachment)
            )
        {
        }

        public Task<Project> GetProjectOnlyByNameAsync(string projectName)
            => Set.SingleOrDefaultAsync(p => p.Name == projectName);

        public Task<Project> GetProjectWithTagsByNameAsync(string projectName)
            => Set.Include(p => p.Tags).SingleOrDefaultAsync(p => p.Name == projectName);

        public Task<Tag> GetTagOnlyByTagIdAsync(int tagId)
            => Set
                .Include(p => p.Tags)
                .SelectMany(project => project.Tags)
                .SingleOrDefaultAsync(tag => tag.Id == tagId);

        public Task<Tag> GetTagOnlyByGuidAsync(Guid guid)
            => Set
                .Include(p => p.Tags)
                .SelectMany(project => project.Tags)
                .SingleOrDefaultAsync(tag => tag.Guid == guid);

        public Task<Tag> GetTagWithPreservationHistoryByTagIdAsync(int tagId)
            => DefaultQuery
                .SelectMany(project => project.Tags)
                .SingleOrDefaultAsync(tag => tag.Id == tagId);

        public Task<Tag> GetTagWithActionsByTagIdAsync(int tagId)
            => Set
                .Include(p => p.Tags)
                    .ThenInclude(t => t.Actions)
                    .ThenInclude(a => a.Attachments)
                .SelectMany(project => project.Tags)
                .SingleOrDefaultAsync(tag => tag.Id == tagId);

        public Task<Tag> GetTagWithAttachmentsByTagIdAsync(int tagId)
            => Set
                .Include(p => p.Tags)
                    .ThenInclude(t => t.Attachments)
                .SelectMany(project => project.Tags)
                .SingleOrDefaultAsync(tag => tag.Id == tagId);

        public Task<List<Tag>> GetTagsOnlyByTagIdsAsync(IEnumerable<int> tagIds)
            => Set
                .Include(p => p.Tags)
                .SelectMany(project => project.Tags)
                .Where(tag => tagIds.Contains(tag.Id))
                .ToListAsync();

        public Task<List<Tag>> GetTagsWithPreservationHistoryByTagIdsAsync(IEnumerable<int> tagIds)
            => DefaultQuery
                .SelectMany(project => project.Tags)
                .Where(tag => tagIds.Contains(tag.Id))
                .ToListAsync();

        public Task<List<Tag>> GetStandardTagsInProjectOnlyAsync(string projectName)
            => Set.Where(project => project.Name == projectName)
                .SelectMany(project => project.Tags)
                .Where(tag => tag.TagType == TagType.Standard)
                .ToListAsync();

        public void RemoveTag(Tag tag)
        {
            if (tag.Actions.Any())
            {
                throw new Exception("Can't remove Tag with any actions");
            }
            if (tag.Attachments.Any())
            {
                throw new Exception("Can't remove Tag with any attachments");
            }
            foreach (var tagRequirement in tag.Requirements)
            {
                _context.TagRequirements.Remove(tagRequirement);
            }
            _context.Tags.Remove(tag);
        }

        public Task<List<Tag>> GetStandardTagsInProjectInStepsAsync(string projectName, IEnumerable<string> tagNos, IEnumerable<int> stepIds)
            => Set.Where(project => project.Name == projectName)
                .SelectMany(project => project.Tags)
                .Where(tag => tag.TagType == TagType.Standard && tagNos.Contains(tag.TagNo) && stepIds.Contains(tag.StepId))
                .ToListAsync();

        public Task<Project> GetProjectAndTagWithPreservationHistoryByTagIdAsync(int tagId)
            => DefaultQuery
                .Where(project => project.Tags.Any(tag => tag.Id == tagId))
                .SingleOrDefaultAsync();

        public Task<Project> GetProjectOnlyByTagIdAsync(int tagId)
            => Set
                .Where(project => project.Tags.Any(tag => tag.Id == tagId))
                .SingleOrDefaultAsync();

        public Task<Project> GetProjectOnlyByTagGuidAsync(Guid tagGuid)
            => Set
                .Where(project => project.Tags.Any(tag => tag.Guid == tagGuid))
                .SingleOrDefaultAsync();
    }
}
