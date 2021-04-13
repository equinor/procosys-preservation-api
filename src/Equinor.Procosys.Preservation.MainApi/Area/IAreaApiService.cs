using System.Threading.Tasks;

namespace Equinor.ProCoSys.Preservation.MainApi.Area
{
    public interface IAreaApiService
    {
        Task<ProcosysArea> TryGetAreaAsync(string plant, string code);
    }
}
