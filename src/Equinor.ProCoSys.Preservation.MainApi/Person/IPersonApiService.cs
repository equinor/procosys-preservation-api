using System;
using System.Threading.Tasks;

namespace Equinor.ProCoSys.Preservation.MainApi.Person
{
    public interface IPersonApiService
    {
        Task<ProCoSysPerson> TryGetPersonByOidAsync(Guid azureOid);
    }
}
