using System;
using System.Threading.Tasks;

namespace Equinor.ProCoSys.Preservation.MainApi.Person
{
    public interface IPersonApiService
    {
        Task<PCSPerson> TryGetPersonByOidAsync(Guid azureOid);
    }
}
