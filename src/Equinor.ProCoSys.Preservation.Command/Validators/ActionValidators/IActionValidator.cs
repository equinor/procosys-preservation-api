using System.Threading;
using System.Threading.Tasks;

namespace Equinor.ProCoSys.Preservation.Command.Validators.ActionValidators
{
    public interface IActionValidator
    {
        Task<bool> IsClosedAsync(int actionId, CancellationToken token);
        Task<bool> AttachmentWithFilenameExistsAsync(int actionId, string fileName, CancellationToken token);
    }
}
