using System.Collections.Generic;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.TagFunctionAggregate
{
    public interface ITagFunctionRepository : IRepository<TagFunction>
    {
        Task<TagFunction> GetByCodesAsync(string tagFunctionCode, string registerCode);
        Task<List<TagFunction>> GetAllWithRequirementsAsync();
    }
}
