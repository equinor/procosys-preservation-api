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

        public Task<Journey> GetJourneyByStepIdAsync(int stepId) =>
            DefaultQuery
                .Where(journey => journey.Steps.Any(s => s.Id == stepId))
                .FirstOrDefaultAsync();

        public Task<Step> GetStepByStepIdAsync(int stepId)
            => DefaultQuery
                .SelectMany(j => j.Steps)
                .FirstOrDefaultAsync(s => s.Id == stepId);

        public Task<List<Step>> GetStepsByModeIdAsync(int modeId)
            => DefaultQuery
                .SelectMany(j => j.Steps)
                .Where(s => s.ModeId == modeId)
                .ToListAsync();

        public Task<Journey> GetByTitleAsync(string title) =>
            DefaultQuery
            .FirstOrDefaultAsync(j => j.Title == title);
    }
}
