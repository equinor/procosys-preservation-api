using System.Linq;

namespace Equinor.ProCoSys.Preservation.Domain
{
    public interface IReadOnlyContext
    {
        IQueryable<TEntity> QuerySet<TEntity>() where TEntity : class;
    }
}
