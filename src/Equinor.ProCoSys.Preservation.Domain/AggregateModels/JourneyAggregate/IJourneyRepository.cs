using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate
{
    public interface IJourneyRepository : IRepository<Journey>
    {
        Task<Step> GetStepByStepIdAsync(int stepId);
        Task<List<Journey>> GetJourneysByStepIdsAsync(IEnumerable<int> stepIds);
        Task<List<Journey>> GetJourneysWithAutoTransferStepsAsync(AutoTransferMethod autoTransferMethod);
        void RemoveStep(Step step);
    }
}
