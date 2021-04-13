using System.Threading.Tasks;

namespace Equinor.ProCoSys.Preservation.MainApi.Responsible
{
    public interface IResponsibleApiService
    {
        Task<ProcosysResponsible> TryGetResponsibleAsync(string plant, string code);
    }
}
