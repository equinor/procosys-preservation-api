using System.Collections.Generic;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.MainApi.Discipline
{
    public interface IDisciplineApiService
    {
        Task<List<ProcosysDiscipline>> GetDisciplinesAsync(string plant);
        Task<ProcosysDiscipline> GetDisciplineAsync(string plant, string code);
    }
}
