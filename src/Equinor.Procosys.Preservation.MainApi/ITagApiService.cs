using System.Collections.Generic;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.MainApi
{
    public interface ITagApiService
    {
        Task<ProcosysTagDetails> GetTagDetails(string plant, string projectName, string tagNumber);
        Task<IEnumerable<ProcosysTagOverview>> GetTags(string plant, string projectName, string searchString);
    }
}
