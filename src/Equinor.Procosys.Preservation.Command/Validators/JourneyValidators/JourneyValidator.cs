using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.Procosys.Preservation.Command.Validators.JourneyValidators
{
    public class JourneyValidator : IJourneyValidator
    {
        private readonly IReadOnlyContext _context;

        public JourneyValidator(IReadOnlyContext context) => _context = context;

        public async Task<bool> ExistsAsync(int journeyId, CancellationToken token) =>
            await (from j in _context.QuerySet<Journey>()
                where j.Id == journeyId
                select j).AnyAsync(token);

        public async Task<bool> ExistsWithSameTitleAsync(string journeyTitle, CancellationToken token) =>
            await (from j in _context.QuerySet<Journey>()
                where j.Title == journeyTitle
                select j).AnyAsync(token);

        public async Task<bool> ExistsWithSameTitleInAnotherJourneyAsync(int journeyId, string journeyTitle, CancellationToken token) =>
            await (from j in _context.QuerySet<Journey>()
                where j.Id != journeyId && j.Title == journeyTitle
                select j).AnyAsync(token);

        public async Task<bool> IsVoidedAsync(int journeyId, CancellationToken token)
        {
            var journey = await (from j in _context.QuerySet<Journey>()
                where j.Id == journeyId
                select j).SingleOrDefaultAsync(token);
            return journey != null && journey.IsVoided;
        }

        public async Task<bool> AreAdjacentStepsInAJourneyAsync(int journeyId, int stepAId, int stepBId, CancellationToken token)
        {
            var journey = await (from j in _context.QuerySet<Journey>().Include(j => j.Steps)
                where j.Id == journeyId
                select j).SingleOrDefaultAsync(token);

            return journey.AreAdjacentSteps(stepAId, stepBId);
        }

        public async Task<bool> IsFirstStepIfModeIsForSupplier(int journeyId, int modeId, CancellationToken token)
        {
            var journey = await _context.QuerySet<Journey>()
                .Include(j => j.Steps)
                .SingleAsync(j => j.Id == journeyId, token);

            if (!journey.Steps.Any())
            {
                return true;
            }

            var mode = await _context.QuerySet<Mode>().SingleAsync(m => m.Id == modeId, token);
            return mode.ForSupplier == false;
        }
    }
}
