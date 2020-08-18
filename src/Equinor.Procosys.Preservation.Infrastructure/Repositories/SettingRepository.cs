using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.SettingAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.Procosys.Preservation.Infrastructure.Repositories
{
    public class SettingRepository : RepositoryBase<Setting>, ISettingRepository
    {
        public SettingRepository(PreservationContext context) : base(context, context.Settings)
        {
        }
        
        public Task<Setting> GetByCodeAsync(string SettingCode) 
            => DefaultQuery.SingleOrDefaultAsync(r => r.Code == SettingCode);
    }
}
