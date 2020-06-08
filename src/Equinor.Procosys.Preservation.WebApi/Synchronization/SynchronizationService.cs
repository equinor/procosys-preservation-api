using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Infrastructure;

namespace Equinor.Procosys.Preservation.WebApi.Synchronization
{
    public class SynchronizationService : ISynchronizationService
    {
        private readonly PreservationContext _context;

        public SynchronizationService(PreservationContext context)
        {
            _context = context;
        }

        public Task Synchronize(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }
}
