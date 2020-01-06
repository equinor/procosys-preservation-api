using System.Collections.Generic;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.MainApi
{
    public interface ITagApiService
    {
        Task<IEnumerable<MainTagDto>> GetTags(string plant, string searchString);
    }
}
