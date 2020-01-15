using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate
{
    public interface ITagRepository : IRepository<Tag>
    {
        Task<Tag> GetByNoAsync(string tagNo, string projectNo);
    }
}
