using System.Collections.Generic;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.MainApi.Tag
{
    public interface ITagApiService
    {
        Task<ProcosysTagDetails> GetTagDetails(string plant, string projectName, string tagNumber);
        Task<IList<ProcosysTagOverview>> GetTags(string plant, string projectName, string searchString);
    }
}
