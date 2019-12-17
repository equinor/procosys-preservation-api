using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.Procosys.Preservation.Infrastructure.Repositories
{
    public class JourneyRepository : RepositoryBase<Journey>, IJourneyRepository
    {
        public JourneyRepository(PreservationContext context)
            : base(context.Set<Journey>(), context)
        {
        }

        public override Task<Journey> GetByIdAsync(int id) =>
                Set
                .Include(x => x.Steps)
                .FirstOrDefaultAsync(x => x.Id == id);

        public Task<Journey> GetByTitleAsync(string title) =>
            Set.FirstOrDefaultAsync(x => x.Title == title);
    }
}
