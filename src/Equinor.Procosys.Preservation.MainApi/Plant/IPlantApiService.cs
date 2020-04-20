using System.Collections.Generic;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.MainApi.Plant
{
    public interface IPlantApiService
    {
        Task<List<ProcosysPlant>> GetPlantsAsync();
        Task<bool> IsPlantValidAsync(string plant);
    }
}
