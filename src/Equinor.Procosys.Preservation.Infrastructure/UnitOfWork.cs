using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;

namespace Equinor.Procosys.Preservation.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PreservationContext _context;

        public UnitOfWork(PreservationContext context) => _context = context;

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
