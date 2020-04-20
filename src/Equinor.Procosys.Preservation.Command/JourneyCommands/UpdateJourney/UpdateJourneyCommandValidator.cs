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
                .MustAsync((command, token) => HaveUniqueJourneyTitleAsync(command.Title, token))
                .WithMessage(command => $"Another journey with this title already exists! Journey={command.Title}");

            async Task<bool> HaveUniqueJourneyTitleAsync(string journeyTitle, CancellationToken token) =>
                !await journeyValidator.ExistsAsync(journeyTitle, token);
        }
    }
}
