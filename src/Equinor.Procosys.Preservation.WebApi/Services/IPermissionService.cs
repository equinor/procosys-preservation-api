using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.WebApi.Services
{
    public interface IPermissionService
    {
        Task<IList<string>> GetPermissionsForUserOid(Guid userOid);
    }
}
