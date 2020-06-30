using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.MainApi.Area
{
    public interface IAreaApiService
    {
        Task<ProcosysArea> TryGetAreaAsync(string plant, string code);
    }
}
