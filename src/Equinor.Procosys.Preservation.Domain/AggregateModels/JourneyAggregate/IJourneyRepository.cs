using System.Collections.Generic;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate
{
    public interface IJourneyRepository : IRepository<Journey>
    {
        Task<Step> GetStepByStepIdAsync(int stepId);
        
        Task<List<Journey>> GetJourneysByStepIdsAsync(IEnumerable<int> stepIds);
        Task<Journey> GetJourneyByJourneyIdAsync(int journeyId);
    }
}
