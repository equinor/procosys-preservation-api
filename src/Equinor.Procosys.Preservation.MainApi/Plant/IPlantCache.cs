using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.MainApi.Plant
{
    public interface IPlantCache
    {
        Task<IList<string>> GetPlantWithAccessForUserAsync(Guid userOid);
        Task<bool> HasUserAccessToPlantAsync(string plantId, Guid userOid);
        Task<bool> HasCurrentUserAccessToPlantAsync(string plantId);
        Task<bool> IsAValidPlant(string plantId);
        void Clear(Guid userOid);
    }
}
