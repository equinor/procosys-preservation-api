using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.SettingAggregate
{
    public interface ISettingRepository : IRepository<Setting>
    {
        Task<Setting> GetByCodeAsync(string settingCode);
    }
}
