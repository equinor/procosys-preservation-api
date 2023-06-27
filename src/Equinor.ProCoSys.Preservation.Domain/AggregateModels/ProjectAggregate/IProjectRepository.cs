using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public interface IProjectRepository : IRepository<Project>
    {
        Task<Project> GetProjectOnlyByNameAsync(string projectName);
        Task<Tag> GetTagOnlyByTagIdAsync(int tagId);
        Task<Tag> GetTagWithPreservationHistoryByTagIdAsync(int tagId);
        Task<Tag> GetTagWithActionsByTagIdAsync(int tagId);
        Task<Tag> GetTagWithAttachmentsByTagIdAsync(int tagId);
        Task<List<Tag>> GetTagsWithPreservationHistoryByTagIdsAsync(IEnumerable<int> tagIds);
        Task<List<Tag>> GetTagsOnlyByTagIdsAsync(IEnumerable<int> tagIds);
        Task<List<Tag>> GetStandardTagsInProjectOnlyAsync(string projectName);
        void RemoveTag(Tag tag);
        Task<List<Tag>> GetStandardTagsInProjectInStepsAsync(string projectName, IEnumerable<string> tagNos, IEnumerable<int> stepIds);
        Task<Project> GetProjectAndTagWithPreservationHistoryByTagIdAsync(int tagId);
        Task<Project> GetProjectOnlyByTagIdAsync(int tagId);
        Task<Project> GetProjectWithTagsByNameAsync(string projectName);
        Task<Tag> GetTagOnlyByGuidAsync(Guid guid);
    }
}
