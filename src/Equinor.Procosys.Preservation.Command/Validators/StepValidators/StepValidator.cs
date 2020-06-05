using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
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
                where s.Title == stepTitle && j.Id == journeyId
                select s).AnyAsync(token);

        public async Task<bool> ExistsInExistingJourneyAsync(int stepId, string stepTitle, CancellationToken token)
        {
            var journey = await (from j in _context.QuerySet<Journey>().Include(j => j.Steps)
                join step in _context.QuerySet<Step>() on j.Id equals EF.Property<int>(step, "JourneyId")
                where step.Id == stepId
                select j).SingleOrDefaultAsync(token);
            return journey.Steps.Any(s => s.Id != stepId && s.Title == stepTitle);
        }

        public async Task<bool> IsVoidedAsync(int stepId, CancellationToken token)
        {
            var step = await (from s in _context.QuerySet<Step>()
                where s.Id == stepId
                select s).SingleOrDefaultAsync(token);
            return step != null && step.IsVoided;
        }

        public async Task<bool> IsAnyStepForSupplier(int stepAId, int stepBId, CancellationToken token)
            => await (from s in _context.QuerySet<Step>()
                join mode in _context.QuerySet<Mode>() on s.ModeId equals mode.Id
                where (s.Id == stepAId || s.Id == stepBId) && mode.ForSupplier
                select mode).AnyAsync(token);

        public async Task<bool> IsFirstStepOrModeIsNotForSupplier(int journeyId, int modeId,
            int stepId,
            CancellationToken token)
        {
            var journey = await _context.QuerySet<Journey>()
                .Include(j => j.Steps)
                .SingleAsync(j => j.Id == journeyId, token);

            if (journey.Steps.First().Id == stepId)
            {
                return true;
            }

            var mode = await _context.QuerySet<Mode>().SingleAsync(m => m.Id == modeId, token);
            return !mode.ForSupplier;
        }
    }
}
