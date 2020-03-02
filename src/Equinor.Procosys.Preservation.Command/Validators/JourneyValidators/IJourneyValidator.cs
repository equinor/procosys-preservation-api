using System.Threading;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.Command.Validators.JourneyValidators
{
    public interface IJourneyValidator
    {
        Task<bool> ExistsAsync(int journeyId, CancellationToken token);
        Task<bool> ExistsAsync(string title, CancellationToken token);
        Task<bool> IsVoidedAsync(int journeyId, CancellationToken token);
    }
}
