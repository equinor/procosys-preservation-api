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
                    .Include(p => p.Tags)
                    .ThenInclude(t => t.Requirements)
                    .ThenInclude(r => r.PreservationPeriods)
                    .ThenInclude(pp => pp.FieldValues)
                    .ThenInclude(fv => fv.FieldValueAttachment)
                    .Include(p => p.Tags)
                    .ThenInclude(t => t.Actions)
                    .ThenInclude(a => a.Attachments)
                    .Include(p => p.Tags)
                    .ThenInclude(t => t.Attachments)
            )
        {
        }

        public Task<Project> GetProjectOnlyByNameAsync(string projectName)
            => Set.SingleOrDefaultAsync(p => p.Name == projectName);

        public Task<Tag> GetTagByTagIdAsync(int tagId)
            => DefaultQuery
                .SelectMany(project => project.Tags)
                .SingleOrDefaultAsync(tag => tag.Id == tagId);

        public Task<List<Tag>> GetTagsByTagIdsAsync(IEnumerable<int> tagIds)
            => DefaultQuery
                .SelectMany(project => project.Tags)
                .Where(tag => tagIds.Contains(tag.Id))
                .ToListAsync();

        public Task<List<Project>> GetAllProjectsOnlyAsync()
            => Set.ToListAsync();

        public Task<List<Tag>> GetStandardTagsInProjectOnlyAsync(string projectName)
            => Set.Where(project => project.Name == projectName)
                .SelectMany(project => project.Tags)
                .Where(tag => tag.TagType == TagType.Standard)
                .ToListAsync();

        public void RemoveTag(Tag tag)
        {
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

        public Task<Project> GetProjectByTagIdAsync(int tagId)
            => DefaultQuery
                .Where(project => project.Tags.Any(tag => tag.Id == tagId))
                .SingleOrDefaultAsync();

        public Task<Project> GetProjectOnlyByTagIdAsync(int tagId)
            => Set
                .Where(project => project.Tags.Any(tag => tag.Id == tagId))
                .SingleOrDefaultAsync();
    }
}
