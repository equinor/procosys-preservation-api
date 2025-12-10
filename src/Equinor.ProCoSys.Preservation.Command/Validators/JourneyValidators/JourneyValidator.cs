using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.ProCoSys.Preservation.Command.Validators.JourneyValidators
{
    public class JourneyValidator : IJourneyValidator
    {
        private readonly IReadOnlyContext _context;

        public JourneyValidator(IReadOnlyContext context) => _context = context;

        public async Task<bool> ExistsAsync(int journeyId, CancellationToken token) =>
            await (from j in _context.QuerySet<Journey>()
                   where j.Id == journeyId
                   select j).AnyAsync(token);

        public async Task<bool> ExistsStepAsync(int journeyId, int stepId, CancellationToken token)
        {
            var journey = await GetJourneyWithStepsAsync(journeyId, token);

            return journey != null && journey.Steps.Any(s => s.Id == stepId);
        }

        public async Task<bool> ExistsWithSameTitleAsync(string journeyTitle, CancellationToken token) =>
            await (from j in _context.QuerySet<Journey>()
                   where j.Title == journeyTitle
                   select j).AnyAsync(token);

        public async Task<bool> ExistsWithSameTitleInAnotherJourneyAsync(int journeyId, string journeyTitle, CancellationToken token) =>
            await (from j in _context.QuerySet<Journey>()
                   where j.Id != journeyId && j.Title == journeyTitle
                   select j).AnyAsync(token);


        public async Task<bool> AnyStepExistsWithSameTitleAsync(int journeyId, string stepTitle, CancellationToken token)
        {
            var journey = await GetJourneyWithStepsAsync(journeyId, token);

            return journey != null && journey.Steps.Any(s => s.Title == stepTitle);
        }

        public async Task<bool> OtherStepExistsWithSameTitleAsync(int journeyId, int stepId, string stepTitle, CancellationToken token)
        {
            var journey = await GetJourneyWithStepsAsync(journeyId, token);

            return journey != null && journey.Steps.Any(s => s.Id != stepId && s.Title == stepTitle);
        }

        public async Task<bool> IsVoidedAsync(int journeyId, CancellationToken token)
        {
            var journey = await (from j in _context.QuerySet<Journey>()
                                 where j.Id == journeyId
                                 select j).SingleOrDefaultAsync(token);
            return journey != null && journey.IsVoided;
        }

        public async Task<bool> AreAdjacentStepsInAJourneyAsync(int journeyId, int stepAId, int stepBId, CancellationToken token)
        {
            var journey = await GetJourneyWithStepsAsync(journeyId, token);

            return journey != null && journey.AreAdjacentSteps(stepAId, stepBId);
        }

        public async Task<bool> HasAnyStepsAsync(int journeyId, CancellationToken token)
        {
            var journey = await GetJourneyWithStepsAsync(journeyId, token);

            return journey != null && journey.Steps.Any();
        }

        public async Task<bool> HasAnyStepInJourneyATagAsync(int journeyId, CancellationToken token)
        {
            var journey = await GetJourneyWithStepsAsync(journeyId, token);
            if (journey == null || !journey.Steps.Any())
            {
                return false;
            }

            var stepIds = journey.Steps.Select(s => s.Id);
            var inUse = await (from tag in _context.QuerySet<Tag>()
                               where stepIds.Contains(tag.StepId)
                               select tag).AnyAsync(token);
            return inUse;
        }

        public async Task<bool> ExistsWithDuplicateTitleAsync(int journeyId, CancellationToken token)
        {
            var journey = await (from j in _context.QuerySet<Journey>()
                                 where j.Id == journeyId
                                 select j).SingleOrDefaultAsync(token);
            if (journey == null)
            {
                return false;
            }

            return await (from j in _context.QuerySet<Journey>()
                          where j.Title == $"{journey.Title}{Journey.DuplicatePrefix}"
                          select j).AnyAsync(token);
        }

        public async Task<bool> HasAnyStepWithAutoTransferMethodAsync(int journeyId, AutoTransferMethod autoTransferMethod, CancellationToken token)
        {
            var journey = await GetJourneyWithStepsAsync(journeyId, token);

            return journey != null && journey.Steps.Any(s => s.AutoTransferMethod == autoTransferMethod);
        }

        public async Task<bool> HasOtherStepWithAutoTransferMethodAsync(int journeyId, int stepId, AutoTransferMethod autoTransferMethod, CancellationToken token)
        {
            var journey = await GetJourneyWithStepsAsync(journeyId, token);

            return journey != null && journey.Steps.Any(s => s.AutoTransferMethod == autoTransferMethod && s.Id != stepId);
        }

        private async Task<Journey> GetJourneyWithStepsAsync(int journeyId, CancellationToken token)
        {
            var journey = await _context.QuerySet<Journey>()
                .Include(j => j.Steps)
                .SingleOrDefaultAsync(j => j.Id == journeyId, token);
            return journey;
        }
    }
}
