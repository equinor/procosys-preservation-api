using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.Procosys.Preservation.Infrastructure.Repositories
{
    public class JourneyRepository : RepositoryBase<Journey>, IJourneyRepository
    {
        public JourneyRepository(PreservationContext context)
            : base(context.Set<Journey>())
        {
        }

        public override Task<Journey> GetByIdAsync(int id) =>
                Set
                .Include(x => x.Steps)
                .FirstOrDefaultAsync(x => x.Id == id);

        public Task<Journey> GetByStepId(int stepId) =>
            Set
                .Include(x => x.Steps)
                .Where(journey => journey.Steps.Any(step => step.Id == stepId))
                .FirstOrDefaultAsync();

        public Task<Journey> GetByTitleAsync(string title) =>
            Set
            .Include(x => x.Steps)
            .FirstOrDefaultAsync(x => x.Title == title);
    }
}
