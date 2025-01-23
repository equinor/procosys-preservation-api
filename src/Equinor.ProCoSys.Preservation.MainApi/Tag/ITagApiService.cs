using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Equinor.ProCoSys.Preservation.MainApi.Tag
{
    public interface ITagApiService
    {
        Task<IList<PCSTagDetails>> GetTagDetailsAsync(
            string plant,
            string projectName,
            IList<string> tagNos,
            CancellationToken cancellationToken,
            bool includeVoidedTags = false);
        Task<IList<PCSTagOverview>> SearchTagsByTagNoAsync(
            string plant,
            string projectName,
            string startsWithTagNo,
            CancellationToken cancellationToken);
        Task<IList<PCSPreservedTag>> GetPreservedTagsAsync(
            string plant,
            string projectName,
            CancellationToken cancellationToken);
        Task<IList<PCSTagOverview>> SearchTagsByTagFunctionsAsync(
            string plant,
            string projectName,
            IList<string> tagFunctionCodeRegisterCodePairs,
            CancellationToken cancellationToken);
        Task MarkTagsAsMigratedAsync(string plant, IEnumerable<long> tagIds, CancellationToken cancellationToken);
    }
}
