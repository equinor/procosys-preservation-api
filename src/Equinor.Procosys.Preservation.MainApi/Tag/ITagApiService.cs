using System.Collections.Generic;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.MainApi.Tag
{
    public interface ITagApiService
    {
        Task<IList<ProcosysTagDetails>> GetTagDetails(string plant, string projectName, IEnumerable<string> tagNos);
        Task<IList<ProcosysTagOverview>> GetTagsByTagNo(string plant, string projectName, string startsWithTagNo);
        Task<IList<ProcosysTagOverview>> GetTagsByTagFunction(string plant, string projectName, string tagFunctionCode, string registerCode);
    }
}
