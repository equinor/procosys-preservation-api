using System.Collections.Generic;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.MainApi.Area
{
    public interface IAreaApiService
    {
        Task<List<ProcosysArea>> GetAreasAsync(string plant);
        Task<ProcosysArea> GetAreaAsync(string plant, string code);
    }
}
