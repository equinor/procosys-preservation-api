using System.Threading.Tasks;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate
{
    public interface IResponsibleRepository : IRepository<Responsible>
    {
        Task<Responsible> GetByCodeAsync(string responsibleCode);
    }
}
