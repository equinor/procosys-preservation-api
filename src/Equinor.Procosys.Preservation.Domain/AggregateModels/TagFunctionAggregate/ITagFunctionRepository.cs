using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.TagFunctionAggregate
{
    public interface ITagFunctionRepository : IRepository<TagFunction>
    {
        Task<TagFunction> GetByNameAsync(string tagFunctionCode, string registerCode);
    }
}
