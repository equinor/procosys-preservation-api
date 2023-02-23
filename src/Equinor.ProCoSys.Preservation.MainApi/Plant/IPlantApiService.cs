using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Equinor.ProCoSys.Preservation.MainApi.Plant
{
    public interface IPlantApiService
    {
        Task<List<ProCoSysPlant>> GetAllPlantsForUserAsync(Guid azureOid);
    }
}
