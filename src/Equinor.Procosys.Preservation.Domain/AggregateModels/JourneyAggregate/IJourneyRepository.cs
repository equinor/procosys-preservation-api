using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate
{
    public interface IJourneyRepository : IRepository<Journey>
    {
        Task<Journey> GetByTitleAsync(string title);
    }
}
