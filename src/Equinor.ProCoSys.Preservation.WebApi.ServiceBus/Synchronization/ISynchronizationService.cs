using System.Threading;
using System.Threading.Tasks;

namespace Equinor.ProCoSys.Preservation.WebApi.ServiceBus.Synchronization
{
    public interface ISynchronizationService
    {
        Task Synchronize(CancellationToken cancellationToken);
    }
}
