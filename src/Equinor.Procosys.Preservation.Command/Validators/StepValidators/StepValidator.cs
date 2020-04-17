using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.Procosys.Preservation.Command.Validators.StepValidators
{
    public class StepValidator : IStepValidator
    {
        private readonly IReadOnlyContext _context;

        public StepValidator(IReadOnlyContext context) => _context = context;

        public async Task<bool> ExistsAsync(int stepId, CancellationToken token)
            => await (from s in _context.QuerySet<Step>()
                where s.Id == stepId
                select s).AnyAsync(token);

        public async Task<bool> ExistsAsync(int journeyId, string stepTitle, CancellationToken token)
            => await (from s in _context.QuerySet<Step>()
                join j in _context.QuerySet<Journey>() on EF.Property<int>(s, "JourneyId") equals j.Id
                where s.Title == stepTitle && s.Id == journeyId
                select s).AnyAsync(token);

        public async Task<bool> IsVoidedAsync(int stepId, CancellationToken token)
        {
            var step = await (from s in _context.QuerySet<Step>()
                where s.Id == stepId
                select s).SingleOrDefaultAsync(token);
            return step != null && step.IsVoided;
        }
    }
}
