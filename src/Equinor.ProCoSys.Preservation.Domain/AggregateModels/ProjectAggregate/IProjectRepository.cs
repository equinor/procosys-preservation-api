using System.Collections.Generic;
using System.Threading.Tasks;

namespace Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public interface IProjectRepository : IRepository<Project>
    {
        Task<Project> GetProjectOnlyByNameAsync(string projectName);
        Task<Tag> GetTagByTagIdAsync(int tagId);
        Task<List<Tag>> GetTagsByTagIdsAsync(IEnumerable<int> tagIds);
        Task<List<Tag>> GetStandardTagsInProjectOnlyAsync(string projectName);
        void RemoveTag(Tag tag);
        Task<List<Tag>> GetStandardTagsInProjectInStepsAsync(string projectName, IEnumerable<string> tagNos, IEnumerable<int> stepIds);
        Task<Project> GetProjectByTagIdAsync(int tagId);
        Task<Project> GetProjectOnlyByTagIdAsync(int tagId);
        Task<Project> GetProjectWithTagsByNameAsync(string projectName);
    }
}
