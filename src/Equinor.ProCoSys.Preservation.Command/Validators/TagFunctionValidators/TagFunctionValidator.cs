using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.TagFunctionAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.ProCoSys.Preservation.Command.Validators.TagFunctionValidators
{
    public class TagFunctionValidator : ITagFunctionValidator
    {
        private readonly IReadOnlyContext _context;

        public TagFunctionValidator(IReadOnlyContext context) => _context = context;

        public async Task<bool> ExistsAsync(string tagFunctionCode, CancellationToken token) =>
            await (from tf in _context.QuerySet<TagFunction>()
                where tf.Code == tagFunctionCode
                   select tf).AnyAsync(token);

        public async Task<bool> IsVoidedAsync(string tagFunctionCode, CancellationToken token)
        {
            var tagFunction = await (from tf in _context.QuerySet<TagFunction>()
                where tf.Code == tagFunctionCode
                select tf).SingleOrDefaultAsync(token);
            return tagFunction != null && tagFunction.IsVoided;
        }
    }
}
