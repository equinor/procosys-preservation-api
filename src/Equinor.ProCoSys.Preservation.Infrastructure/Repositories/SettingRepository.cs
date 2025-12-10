using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.SettingAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.ProCoSys.Preservation.Infrastructure.Repositories
{
    public class SettingRepository : RepositoryBase<Setting>, ISettingRepository
    {
        public SettingRepository(PreservationContext context) : base(context, context.Settings)
        {
        }

        public Task<Setting> GetByCodeAsync(string settingCode)
            => DefaultQuery.SingleOrDefaultAsync(r => r.Code == settingCode);
    }
}
