using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.Procosys.Preservation.Infrastructure.Repositories
{
    public class ModeRepository : RepositoryBase<Mode>, IModeRepository
    {
        public ModeRepository(PreservationContext context)
            : base(context.Modes)
        {
        }

        public Task<Mode> GetByTitleAsync(string title)
            => DefaultQuery
                .FirstOrDefaultAsync(j => j.Title == title);
    }
}
