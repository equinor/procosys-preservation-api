using System.Collections.Generic;
using System.Threading.Tasks;

namespace Equinor.ProCoSys.Preservation.MainApi.Tag
{
    public interface ITagApiService
    {
        Task<IList<PCSTagDetails>> GetTagDetailsAsync(string plant, string projectName, IList<string> tagNos, bool includeVoidedTags = false);
        Task<IList<PCSTagOverview>> SearchTagsByTagNoAsync(string plant, string projectName, string startsWithTagNo);
        Task<IList<PCSPreservedTag>> GetPreservedTagsAsync(string plant, string projectName);
        Task<IList<PCSTagOverview>> SearchTagsByTagFunctionsAsync(string plant, string projectName, IList<string> tagFunctionCodeRegisterCodePairs);
        Task MarkTagsAsMigratedAsync(string plant, IEnumerable<long> tagIds);
    }
}
