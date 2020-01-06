using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.Procosys.Preservation.Infrastructure.Repositories
{
    public class RequirementTypeRepository : RepositoryBase<RequirementType>, IRequirementTypeRepository
    {
        public RequirementTypeRepository(PreservationContext context)
            : base(context.RequirementTypes)
        {
        }

        public override Task<RequirementType> GetByIdAsync(int id) =>
            Set.Include(x => x.RequirementDefinitions)
                .ThenInclude(x => x.Fields)
                .FirstOrDefaultAsync(x => x.Id == id);

        public override Task<List<RequirementType>> GetAllAsync() =>
            Set.Include(x => x.RequirementDefinitions)
                .ThenInclude(x => x.Fields).ToListAsync();
    }
}
