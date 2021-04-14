using System.Threading.Tasks;

namespace Equinor.ProCoSys.Preservation.Domain.AggregateModels.SettingAggregate
{
    public interface ISettingRepository : IRepository<Setting>
    {
        Task<Setting> GetByCodeAsync(string settingCode);
    }
}
