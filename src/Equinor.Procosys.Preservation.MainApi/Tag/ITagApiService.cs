using System.Collections.Generic;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.MainApi.Tag
{
    public interface ITagApiService
    {
        // todo rename all async methods to .. Async
        Task<IList<ProcosysTagDetails>> GetTagDetails(string plant, string projectName, IEnumerable<string> tagNos);
        Task<IList<ProcosysTagOverview>> SearchTagsByTagNo(string plant, string projectName, string startsWithTagNo);
        Task<IList<ProcosysTagOverview>> SearchTagsByTagFunction(string plant, string projectName, string tagFunctionCode, string registerCode);
    }
}
