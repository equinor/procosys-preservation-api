using System.Linq;

namespace Equinor.Procosys.Preservation.Domain
{
    public interface IReadOnlyContext
    {
        IQueryable<TQuery> ReadOnlySet<TQuery>() where TQuery : class;
    }
}
