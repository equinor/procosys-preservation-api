using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate
{
    public interface IResponsibleRepository : IRepository<Responsible>
    {
        Task<Responsible> GetByCodeAsync(string responsibleCode);
    }
}
