using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.Procosys.Preservation.Infrastructure.Repositories
{
    public class JourneyRepository : RepositoryBase<Journey>, IJourneyRepository
    {
        public JourneyRepository(PreservationContext context)
            : base(context.Set<Journey>(), context.Set<Journey>().Include(j => j.Steps))
        {
        }

        public Task<Journey> GetJourneyByStepIdAsync(int stepId) =>
            DefaultQuery
                .Where(journey => journey.Steps.Any(s => s.Id == stepId))
                .FirstOrDefaultAsync();

        public Task<Step> GetStepByStepIdAsync(int stepId)
            => DefaultQuery
                .Where(journey => journey.Steps.Any(s => s.Id == stepId))
                .SelectMany(j => j.Steps)
                .Where(s => s.Id == stepId)
                .FirstOrDefaultAsync();

        public Task<Journey> GetByTitleAsync(string title) =>
            DefaultQuery
            .FirstOrDefaultAsync(j => j.Title == title);
    }
}
