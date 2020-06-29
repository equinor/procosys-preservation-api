using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.MainApi.Responsible
{
    public interface IResponsibleApiService
    {
        Task<ProcosysResponsible> TryGetResponsibleAsync(string plant, string code);
    }
}
