using System.Threading.Tasks;

namespace Equinor.ProCoSys.Preservation.MainApi.Discipline
{
    public interface IDisciplineApiService
    {
        Task<ProcosysDiscipline> TryGetDisciplineAsync(string plant, string code);
    }
}
