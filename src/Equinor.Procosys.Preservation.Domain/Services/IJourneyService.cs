using System.Threading;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.Domain.Services
{
    public interface IJourneyService
    {
        Task<bool> IsJourneyInUseAsync(long journeyId, CancellationToken cancellationToken);
    }
}
