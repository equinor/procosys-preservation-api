using System.Collections.Generic;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.MainApi.Area
{
    public interface IAreaApiService
    {
        Task<List<ProcosysArea>> GetAreas(string plant);
    }
}
