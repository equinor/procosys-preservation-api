using System;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate
{
    public interface IPersonRepository : IRepository<Person>
    {
        Task<Person> GetByOidAsync(Guid oid);
        Task<Person> GetWithSavedFiltersByOidAsync(Guid oid);
        void RemoveSavedFilter(SavedFilter savedFilter);
    }
}
