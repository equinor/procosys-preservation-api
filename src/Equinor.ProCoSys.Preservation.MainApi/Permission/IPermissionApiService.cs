using System.Collections.Generic;
using System.Threading.Tasks;

namespace Equinor.ProCoSys.Preservation.MainApi.Permission
{
    public interface IPermissionApiService
    {
        Task<List<string>> GetPermissionsAsync(string plantId);
        Task<List<ProCoSysProject>> GetAllOpenProjectsAsync(string plantId);
        Task<List<string>> GetContentRestrictionsAsync(string plantId);
    }
}
