using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.Procosys.Preservation.Command.Validators.RequirementTypeValidators
{
    public class RequirementTypeValidator : IRequirementTypeValidator
    {
        private readonly IReadOnlyContext _context;

        public RequirementTypeValidator(IReadOnlyContext context) => _context = context;

        public async Task<bool> ExistsAsync(int requirementTypeId, CancellationToken token) =>
            await (from rt in _context.QuerySet<RequirementType>()
                where rt.Id == requirementTypeId
                select rt).AnyAsync(token);

        public async Task<bool> IsVoidedAsync(int requirementTypeId, CancellationToken token)
        {
            var reqType = await (from rt in _context.QuerySet<RequirementType>()
                where rt.Id == requirementTypeId
                select rt).SingleOrDefaultAsync(token);
            return reqType != null && reqType.IsVoided;
        }
        public async Task<bool> IsNotUniqueCodeAsync(string code, CancellationToken token) =>
            await (from rt in _context.QuerySet<RequirementType>()
                where rt.Code == code
                select rt).AnyAsync(token);

        public async Task<bool> IsNotUniqueTitleAsync(string title, CancellationToken token) =>
            await (from rt in _context.QuerySet<RequirementType>()
                where rt.Title == title
                select rt).AnyAsync(token);
    }
}
