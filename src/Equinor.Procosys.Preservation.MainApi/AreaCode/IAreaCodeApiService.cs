using System.Collections.Generic;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.MainApi.AreaCode
{
    public interface IAreaCodeApiService
    {
        Task<List<ProcosysAreaCode>> GetAreaCodes(string plant);
    }
}
