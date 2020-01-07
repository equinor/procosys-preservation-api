using System.Collections.Generic;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.MainApi
{
    public interface IPlantApiService
    {
        Task<IEnumerable<ProcosysPlant>> GetPlants();
        Task<bool> IsPlantValidAsync(string plant);
    }
}
