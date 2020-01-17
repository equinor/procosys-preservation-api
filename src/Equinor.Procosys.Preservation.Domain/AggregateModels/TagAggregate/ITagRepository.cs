using System.Collections.Generic;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate
{
    public interface ITagRepository : IRepository<Tag>
    {
        Task<List<Tag>> GetAllByProjectNoAsync(string projectNo);
        Task<Tag> GetByNoAsync(string tagNo, string projectNo);
    }
}
