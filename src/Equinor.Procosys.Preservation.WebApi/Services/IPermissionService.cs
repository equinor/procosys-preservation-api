using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.WebApi.Services
{
    public interface IPermissionService
    {
        Task<IList<string>> GetPlantIdsForUserOidAsync(Guid userOid);
        Task<IList<string>> GetPermissionsForUserOidAsync(Guid userOid);
        Task<IList<string>> GetProjectNamesForUserOidAsync(Guid userOid);
        Task<IList<string>> GetContentRestrictionsForUserOidAsync(Guid userOid);
    }
}
