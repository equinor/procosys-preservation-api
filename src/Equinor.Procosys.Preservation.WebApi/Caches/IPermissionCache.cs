using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.WebApi.Caches
{
    public interface IPermissionCache
    {
        Task<IList<string>> GetPermissionsForUserAsync(string plantId, Guid userOid);
        Task<IList<string>> GetProjectNamesForUserOidAsync(string plantId, Guid userOid);
        Task<IList<string>> GetContentRestrictionsForUserOidAsync(string plantId, Guid userOid);
    }
}
