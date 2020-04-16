using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.Procosys.Preservation.Infrastructure.Repositories
{
    public class JourneyRepository : RepositoryBase<Journey>, IJourneyRepository
    {
        public JourneyRepository(PreservationContext context)
            : base(context.Journeys, context.Journeys.Include(j => j.Steps))
        {
        }

        public Task<Step> GetStepByStepIdAsync(int stepId)
            => DefaultQuery
                .SelectMany(j => j.Steps)
                .SingleOrDefaultAsync(s => s.Id == stepId);

        public Task<List<Journey>> GetJourneysByStepIdsAsync(IEnumerable<int> stepIds)
        => DefaultQuery
            .Where(journey => journey.Steps.Any(s => stepIds.Contains(s.Id)))
            .ToListAsync();
    }
}
