using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Equinor.ProCoSys.Preservation.MainApi.Plant
{
    public interface IPlantCache
    {
        Task<IList<string>> GetPlantIdsWithAccessForUserAsync(Guid userOid);
        Task<bool> HasUserAccessToPlantAsync(string plantId, Guid userOid);
        Task<bool> HasCurrentUserAccessToPlantAsync(string plantId);
        Task<bool> IsAValidPlantAsync(string plantId);
        Task<string> GetPlantTitleAsync(string plantId);
        void Clear(Guid userOid);
    }
}
