using System;
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

        public async Task<bool> AreAdjacentStepsInAJourneyAsync(int journeyId, int stepAId, int stepBId, CancellationToken token)
        {
            var stepA = await (from s in _context.QuerySet<Step>()
                where s.Id == stepAId
                select s).SingleOrDefaultAsync(token);

            var stepB = await (from s in _context.QuerySet<Step>()
                where s.Id == stepBId
                select s).SingleOrDefaultAsync(token);

            var minKey = Math.Min(stepA.SortKey, stepB.SortKey);
            var maxKey = Math.Max(stepA.SortKey, stepB.SortKey);

            var step = await (from s in _context.QuerySet<Step>()
                where s.SortKey.CompareTo(minKey) > 0 &&
                      maxKey.CompareTo(s.SortKey) > 0 &&
                      EF.Property<int>(s, "JourneyId") == journeyId
                              select s).SingleOrDefaultAsync(token);

            return step != null;
        }
    }
}
