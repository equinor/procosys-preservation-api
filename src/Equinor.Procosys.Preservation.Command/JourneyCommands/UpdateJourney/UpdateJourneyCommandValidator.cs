using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.JourneyValidators;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.UpdateJourney
{
    public class UpdateJourneyCommandValidator : AbstractValidator<UpdateJourneyCommand>
    {
        public UpdateJourneyCommandValidator(IJourneyValidator journeyValidator)
        {
            RuleFor(command => command)
                .MustAsync((command, token) => BeAnExistingJourney(command.JourneyId, token))
                .WithMessage(command => $"Journey doesn't exists! Journey={command.JourneyId}")
                .MustAsync((command, token) => HaveUniqueJourneyTitleAsync(command.JourneyId, command.Title, token))
                .WithMessage(command => $"Another journey with this title already exists! Journey={command.Title}");

            async Task<bool> BeAnExistingJourney(int journeyId, CancellationToken token)
                => await journeyValidator.ExistsAsync(journeyId, token);
            async Task<bool> HaveUniqueJourneyTitleAsync(int journeyId, string journeyTitle, CancellationToken token) =>
                !await journeyValidator.ExistsAsync(journeyId, journeyTitle, token);
        }
    }
}
