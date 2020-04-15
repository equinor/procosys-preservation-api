using System.Collections.Generic;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.MainApi.Tag
{
    public interface ITagApiService
    {
        Task<IList<ProcosysTagDetails>> GetTagDetailsAsync(string plant, string projectName, IEnumerable<string> tagNos);
        Task<IList<ProcosysTagOverview>> SearchTagsByTagNoAsync(string plant, string projectName, string startsWithTagNo);
        Task<IList<ProcosysTagOverview>> SearchTagsByTagFunctionsAsync(string plant, string projectName, IList<string> tagFunctionCodeRegisterCodePairs);
    }
}
