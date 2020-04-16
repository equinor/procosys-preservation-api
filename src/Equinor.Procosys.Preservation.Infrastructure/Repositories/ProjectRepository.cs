using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.Procosys.Preservation.Infrastructure.Repositories
{
    public class ProjectRepository : RepositoryBase<Project>, IProjectRepository
    {
        public ProjectRepository(PreservationContext context)
            : base(context.Projects, 
                context.Projects
                    .Include(p => p.Tags)
                        .ThenInclude(t => t.Requirements)
                        .ThenInclude(r => r.PreservationPeriods)
                        .ThenInclude(pp => pp.FieldValues)
                    .Include(p => p.Tags)
                        .ThenInclude(t => t.Actions))
        {
        }

        public Task<Project> GetByNameAsync(string projectName)
            => DefaultQuery.FirstOrDefaultAsync(p => p.Name == projectName);

        public Task<List<Tag>> GetAllTagsInProjectAsync(string projectName)
            => DefaultQuery
                .Where(p => p.Name == projectName)
                .SelectMany(p => p.Tags)
                .ToListAsync();

        public Task<Tag> GetTagByTagIdAsync(int tagId)
            => DefaultQuery
                .SelectMany(project => project.Tags)
                .SingleOrDefaultAsync(tag => tag.Id == tagId);

        public Task<List<Tag>> GetTagsByTagIdsAsync(IEnumerable<int> tagIds)
            => DefaultQuery
                .SelectMany(project => project.Tags)
                .Where(tag => tagIds.Contains(tag.Id))
                .ToListAsync();
    }
}
