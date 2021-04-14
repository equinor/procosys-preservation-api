using System.Threading.Tasks;

namespace Equinor.ProCoSys.Preservation.MainApi.TagFunction
{
    public interface ITagFunctionApiService
    {
        Task<PCSTagFunction> TryGetTagFunctionAsync(string plant, string tagFunctionCode, string registerCode);
    }
}
