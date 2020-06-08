using System.Threading;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.WebApi.Synchronization
{
    public interface ISynchronizationService
    {
        Task Synchronize(CancellationToken stoppingToken);
    }
}
