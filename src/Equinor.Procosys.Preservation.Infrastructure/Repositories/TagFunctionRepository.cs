using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagFunctionAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.Procosys.Preservation.Infrastructure.Repositories
{
    public class TagFunctionRepository : RepositoryBase<TagFunction>, ITagFunctionRepository
    {
        public TagFunctionRepository(PreservationContext context)
            : base(context.TagFunctions, context.TagFunctions.Include(j => j.Requirements))
        {
        }

        public Task<TagFunction> GetByCodeAsync(string tagFunctionCode, string registerCode)
            => DefaultQuery.SingleOrDefaultAsync(tf => tf.Code == tagFunctionCode && tf.RegisterCode == registerCode);
    }
}
