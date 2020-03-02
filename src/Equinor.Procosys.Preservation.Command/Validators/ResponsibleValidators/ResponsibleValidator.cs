using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.Procosys.Preservation.Command.Validators.ResponsibleValidators
{
    public class ResponsibleValidator : IResponsibleValidator
    {
        private readonly IReadOnlyContext _context;

        public ResponsibleValidator(IReadOnlyContext context) => _context = context;

        public async Task<bool> ExistsAsync(int responsibleId, CancellationToken token) =>
            await (from r in _context.QuerySet<Responsible>()
                where r.Id == responsibleId
                select r).AnyAsync(token);

        public async Task<bool> IsVoidedAsync(int responsibleId, CancellationToken token)
        {
            var responsible = await (from r in _context.QuerySet<Responsible>()
                where r.Id == responsibleId
                select r).FirstOrDefaultAsync(token);
            return responsible != null && responsible.IsVoided;
        }
    }
}
