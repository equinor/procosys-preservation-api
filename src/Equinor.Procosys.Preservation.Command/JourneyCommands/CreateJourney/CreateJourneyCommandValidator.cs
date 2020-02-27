using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.JourneyValidators;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.CreateJourney
{
    public class CreateJourneyCommandValidator : AbstractValidator<CreateJourneyCommand>
    {
        public CreateJourneyCommandValidator(IJourneyValidator journeyValidator)
        {
            RuleFor(command => command)
                .MustAsync((command, token) => HaveUniqueTitleAsync(command.Title, token))
                .WithMessage(command => $"Journey with title already exists! Journey={command.Title}");

            async Task<bool> HaveUniqueTitleAsync(string title, CancellationToken token)
                => !await journeyValidator.ExistsAsync(title, token);
        }
    }
}
