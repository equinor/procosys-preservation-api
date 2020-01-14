using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.Procosys.Preservation.Infrastructure.Repositories
{
    public class TagRepository : RepositoryBase<Tag>, ITagRepository
    {
        public TagRepository(PreservationContext context)
            : base(context.Tags, context.Tags.Include(t => t.Requirements))
        {
        }

        public Task<Tag> GetByNoAsync(string tagNo, string projectNo)
            => DefaultQuery.FirstOrDefaultAsync(t => t.TagNo == tagNo && t.ProjectNumber == projectNo);

    }
}
