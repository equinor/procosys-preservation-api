using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.MainApi.TagFunction
{
    public interface ITagFunctionApiService
    {
        Task<ProcosysTagFunction> GetTagFunctionAsync(string plant, string tagFunctionCode, string registerCode);
    }
}
