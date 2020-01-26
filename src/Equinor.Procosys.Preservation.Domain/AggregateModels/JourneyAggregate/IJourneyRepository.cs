using System.Collections.Generic;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate
{
    public interface IJourneyRepository : IRepository<Journey>
    {
        Task<Journey> GetByTitleAsync(string title);

        Task<Journey> GetJourneyByStepIdAsync(int stepId);
        
        Task<Step> GetStepByStepIdAsync(int stepId);
        
        Task<List<Step>> GetStepsByStepIdsAsync(IEnumerable<int> stepIds);
        
        Task<List<Step>> GetStepsByModeIdAsync(int modeId);
    }
}
