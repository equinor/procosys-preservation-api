using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.Procosys.Preservation.Infrastructure.Repositories
{
    public class ResponsibleRepository : RepositoryBase<Responsible>, IResponsibleRepository
    {
        public ResponsibleRepository(PreservationContext context) : base(context.Responsibles)
        {
        }
        
        // Todo-comment here: Add test in ResponsibleRepositoryTests.cs (implement in next PR)
        public Task<Responsible> GetByCodeAsync(string responsibleCode) 
            => DefaultQuery.SingleOrDefaultAsync(r => r.Code == responsibleCode);
    }
}
