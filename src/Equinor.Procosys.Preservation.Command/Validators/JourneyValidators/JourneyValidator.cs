using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
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
        
        public async Task<bool> StepExistsAsync(int journeyId, int stepId, CancellationToken token)
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

        public async Task<bool> IsInUseAsync(int journeyId, CancellationToken token)
        {
            var journey = await GetJourneyWithStepsAsync(journeyId, token);
            if (journey == null)
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

        public async Task<bool> HasAnyStepWithTransferOnRfccSignAsync(int journeyId, CancellationToken token)
        {
            var journey = await GetJourneyWithStepsAsync(journeyId, token);

            return journey != null && journey.Steps.Any(s => s.TransferOnRfccSign);
        }

        public async Task<bool> HasAnyStepWithTransferOnRfocSignAsync(int journeyId, CancellationToken token)
        {
            var journey = await GetJourneyWithStepsAsync(journeyId, token);

            return journey != null && journey.Steps.Any(s => s.TransferOnRfocSign);
        }

        public async Task<bool> HasOtherStepWithTransferOnRfccSignAsync(int journeyId, int stepId, CancellationToken token)
        {
            var journey = await GetJourneyWithStepsAsync(journeyId, token);

            return journey != null && journey.Steps.Any(s => s.TransferOnRfccSign && s.Id != stepId);
        }

        public async Task<bool> HasOtherStepWithTransferOnRfocSignAsync(int journeyId, int stepId, CancellationToken token)
        {
            var journey = await GetJourneyWithStepsAsync(journeyId, token);

            return journey != null && journey.Steps.Any(s => s.TransferOnRfocSign && s.Id != stepId);
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
