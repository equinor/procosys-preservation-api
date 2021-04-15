using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.ProCoSys.Preservation.Command.Validators.ResponsibleValidators
{
    public class ResponsibleValidator : IResponsibleValidator
    {
        private readonly IReadOnlyContext _context;

        public ResponsibleValidator(IReadOnlyContext context) => _context = context;

        public async Task<bool> ExistsAndIsVoidedAsync(string responsibleCode, CancellationToken cancellationToken)
        {
            var responsible = await (from r in _context.QuerySet<Responsible>()
                where r.Code == responsibleCode
                select r).SingleOrDefaultAsync(cancellationToken);

            return responsible != null && responsible.IsVoided;
        }
    }
}
