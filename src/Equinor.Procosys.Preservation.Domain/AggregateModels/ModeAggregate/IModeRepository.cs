using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate
{
    public interface IModeRepository : IRepository<Mode>
    {
        Task<Mode> GetByTitleAsync(string title);
    }
}
