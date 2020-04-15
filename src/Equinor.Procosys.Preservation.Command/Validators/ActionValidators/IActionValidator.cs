using System.Threading;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.Command.Validators.ActionValidators
{
    public interface IActionValidator
    {
        Task<bool> ExistsAsync(int tagId, int actionId, CancellationToken token);
        Task<bool> IsClosedAsync(int actionId, CancellationToken token);
    }
}
