using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.Procosys.Preservation.Infrastructure.Repositories
{
    public class ResponsibleRepository : RepositoryBase<Responsible>, IResponsibleRepository
    {
        public ResponsibleRepository(PreservationContext context) : base(context.Responsibles, context)
        {
        }
        
        public Task<Responsible> GetByCodeAsync(string responsibleCode) 
            => DefaultQuery.SingleOrDefaultAsync(r => r.Code == responsibleCode);
    }
}
