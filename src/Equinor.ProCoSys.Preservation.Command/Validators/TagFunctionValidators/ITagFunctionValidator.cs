using System.Threading;
using System.Threading.Tasks;

namespace Equinor.ProCoSys.Preservation.Command.Validators.TagFunctionValidators
{
    public interface ITagFunctionValidator
    {
        Task<bool> ExistsAsync(string tagFunctionCode, CancellationToken token);
        Task<bool> IsVoidedAsync(string tagFunctionCode, CancellationToken token);
    }
}
