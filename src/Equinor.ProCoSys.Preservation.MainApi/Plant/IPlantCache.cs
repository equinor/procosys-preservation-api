using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Equinor.ProCoSys.Preservation.MainApi.Plant
{
    public interface IPlantCache
    {
        Task<IList<string>> GetPlantWithAccessForUserAsync(Guid userOid);
        Task<bool> HasUserAccessToPlantAsync(string plantId, Guid userOid);
        Task<bool> HasCurrentUserAccessToPlantAsync(string plantId);
        Task<bool> IsAValidPlantAsync(string plantId);
        void Clear(Guid userOid);
    }
}
