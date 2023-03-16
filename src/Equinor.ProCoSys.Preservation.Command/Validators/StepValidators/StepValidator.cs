using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.ProCoSys.Preservation.Command.Validators.StepValidators
{
    public class StepValidator : IStepValidator
    {
        private readonly IReadOnlyContext _context;

        public StepValidator(IReadOnlyContext context) => _context = context;

        public async Task<bool> ExistsAsync(int stepId, CancellationToken token)
            => await (from s in _context.QuerySet<Step>()
                where s.Id == stepId
                select s).AnyAsync(token);

        public async Task<bool> IsVoidedAsync(int stepId, CancellationToken token)
        {
            var step = await (from s in _context.QuerySet<Step>()
                where s.Id == stepId
                select s).SingleOrDefaultAsync(token);
            return step != null && step.IsVoided;
        }

        public async Task<bool> IsFirstStepOrModeIsNotForSupplierAsync(int journeyId, int modeId,
            int stepId,
            CancellationToken token)
        {
            var journey = await _context.QuerySet<Journey>()
                .Include(j => j.Steps)
                .SingleAsync(j => j.Id == journeyId, token);

            if (journey.OrderedSteps().First().Id == stepId)
            {
                return true;
            }

            var mode = await _context.QuerySet<Mode>().SingleAsync(m => m.Id == modeId, token);
            return !mode.ForSupplier;
        }

        public async Task<bool> IsForSupplierAsync(int stepId, CancellationToken token)
        {
            var mode = await (from s in _context.QuerySet<Step>()
                join m in _context.QuerySet<Mode>() on s.ModeId equals m.Id
                where s.Id == stepId
                select m).SingleOrDefaultAsync(token);
            return mode != null && mode.ForSupplier;
        }

        public async Task<bool> HasModeAsync(int modeId, int stepId, CancellationToken token)
        {
            var step = await (from s in _context.QuerySet<Step>()
                where s.Id == stepId
                select s).SingleOrDefaultAsync(token);
            return step != null && step.ModeId == modeId;
        }
    }
}
