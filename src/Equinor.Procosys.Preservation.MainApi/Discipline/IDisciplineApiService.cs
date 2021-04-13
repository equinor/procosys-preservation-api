using System.Threading.Tasks;

namespace Equinor.ProCoSys.Preservation.MainApi.Discipline
{
    public interface IDisciplineApiService
    {
        Task<PCSDiscipline> TryGetDisciplineAsync(string plant, string code);
    }
}
