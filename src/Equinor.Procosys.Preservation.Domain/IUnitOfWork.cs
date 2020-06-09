using System;
using System.Threading;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.Domain
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(Guid currentUserOid, CancellationToken cancellationToken = default);
    }
}
