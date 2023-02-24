using System;
using System.Threading.Tasks;

namespace Equinor.ProCoSys.Preservation.MainApi.Person
{
    // todo unit tests
    public interface IPersonCache
    {
        Task<bool> ExistsAsync(Guid userOid);
        Task<ProCoSysPerson> GetAsync(Guid userOid);
    }
}
