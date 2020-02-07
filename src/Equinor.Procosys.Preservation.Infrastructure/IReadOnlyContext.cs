using System.Linq;

namespace Equinor.Procosys.Preservation.Infrastructure
{
    public interface IReadOnlyContext
    {
        IQueryable<TEntity> QuerySet<TEntity>() where TEntity : class;
    }
}
