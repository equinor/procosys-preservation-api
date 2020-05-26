using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.MainApi.Responsible
{
    public interface IResponsibleApiService
    {
        Task<ProcosysResponsible> GetResponsibleAsync(string plant, string code);
    }
}
