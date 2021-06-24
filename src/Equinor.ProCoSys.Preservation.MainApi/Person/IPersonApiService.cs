using System.Collections.Generic;
using System.Threading.Tasks;

namespace Equinor.ProCoSys.Preservation.MainApi.Person
{
    public interface IPersonApiService
    {
        Task<IList<PCSPerson>> GetPersonsByOidsAsync(string plant, IList<string> azureOids);
    }
}
