using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.MainApi.Discipline
{
    public interface IDisciplineApiService
    {
        Task<ProcosysDiscipline> GetDisciplineAsync(string plant, string code);
    }
}
