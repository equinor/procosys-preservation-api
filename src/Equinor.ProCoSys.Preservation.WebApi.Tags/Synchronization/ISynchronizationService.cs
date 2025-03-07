using System.Threading;
using System.Threading.Tasks;

namespace Equinor.ProCoSys.Preservation.WebApi.Tags.Synchronization
{
    public interface ISynchronizationService
    {
        Task Synchronize(CancellationToken cancellationToken);
    }
}
