using System.Threading.Tasks;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.AggregateModels.SettingAggregate
{
    public interface ISettingRepository : IRepository<Setting>
    {
        Task<Setting> GetByCodeAsync(string settingCode);
    }
}
