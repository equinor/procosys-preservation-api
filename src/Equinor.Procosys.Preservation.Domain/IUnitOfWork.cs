using System.Threading;
using System.Threading.Tasks;

namespace Equinor.ProCoSys.Preservation.Domain
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
