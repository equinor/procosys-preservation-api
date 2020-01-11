using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.Procosys.Preservation.Infrastructure.Repositories
{
    public class JourneyRepository : RepositoryBase<Journey>, IJourneyRepository
    {
        public JourneyRepository(PreservationContext context)
            : base(context.Set<Journey>(), context.Set<Journey>().Include(x => x.Steps))
        {
        }

        public Task<Journey> GetJourneyByStepIdAsync(int stepId) =>
            DefaultQuery
                .Where(journey => journey.Steps.Any(step => step.Id == stepId))
                .FirstOrDefaultAsync();

        public Step GetStepByStepId(int stepId)
        {
            var journey = GetJourneyByStepIdAsync(stepId).Result;
            return journey
                ?.Steps
                .FirstOrDefault(step => step.Id == stepId);
        }

        public Task<Journey> GetByTitleAsync(string title) =>
            DefaultQuery
            .FirstOrDefaultAsync(x => x.Title == title);
    }
}
