using System.Threading;
using System.Threading.Tasks;

namespace Equinor.ProCoSys.Preservation.Command.Validators.TagFunctionValidators
{
    public interface ITagFunctionValidator
    {
        Task<bool> ExistsAsync(string tagFunctionCode, string registerCode, CancellationToken token);
        Task<bool> IsVoidedAsync(string tagFunctionCode, string registerCode, CancellationToken token);
    }
}
