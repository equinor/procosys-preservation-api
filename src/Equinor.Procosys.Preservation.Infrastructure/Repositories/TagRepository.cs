using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;

namespace Equinor.Procosys.Preservation.Infrastructure.Repositories
{
    public class TagRepository : RepositoryBase<Tag>, ITagRepository
    {
        public TagRepository(PreservationContext context)
            : base(context.Tags, context)
        {
        }
    }
}
