using System.Collections.Generic;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public interface IProjectRepository : IRepository<Project>
    {
        Task<Project> GetByNameAsync(string projectName);
        Task<Project> GetByTagIdAsync(int tagId);
        Task<List<Tag>> GetAllTagsInProjectAsync(string projectName);
        Task<Tag> GetTagByTagIdAsync(int tagId);
        Task<Tag> GetTagByTagNoAsync(string tagNo, string projectName);
        Task<List<Tag>> GetTagsByTagIdsAsync(IEnumerable<int> tagIds);
    }
}
