using System.Collections.Generic;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.MainApi.Tag
{
    public interface ITagApiService
    {
        Task<IList<ProcosysTagDetails>> GetTagDetails(string plant, string projectName, IEnumerable<string> tagNos);
        Task<IList<ProcosysTagOverview>> GetTags(string plant, string projectName, string searchString);
    }
}
