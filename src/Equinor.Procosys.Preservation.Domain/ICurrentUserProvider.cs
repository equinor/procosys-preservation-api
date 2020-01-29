using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;

namespace Equinor.Procosys.Preservation.Domain
{
    public interface ICurrentUserProvider
    {
        Task<Person> GetCurrentUserAsync();
    }
}
