using System.Collections.Generic;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.MainApi.Permission
{
    public interface IPermissionApiService
    {
        Task<IList<string>> GetPermissionsAsync(string plant);
        Task<IList<string>> GetContentRestrictionsAsync(string plant);
    }
}
