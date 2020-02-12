using System.Collections.Generic;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.MainApi.Discipline
{
    public interface IDisciplineApiService
    {
        Task<List<ProcosysDiscipline>> GetDisciplines(string plant);
    }
}
