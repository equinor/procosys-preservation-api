using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.ProCoSys.Preservation.Infrastructure.Repositories
{
    public class ResponsibleRepository : RepositoryBase<Responsible>, IResponsibleRepository
    {
        public ResponsibleRepository(PreservationContext context) : base(context, context.Responsibles)
        {
        }

        public Task<Responsible> GetByCodeAsync(string responsibleCode)
            => DefaultQuery.SingleOrDefaultAsync(r => r.Code == responsibleCode);

        public override void Remove(Responsible entity)
        {
            base.Remove(entity);
            entity.SetRemoved();
        }
    }
}
