using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Validators.JourneyValidators;
using FluentValidation;

namespace Equinor.ProCoSys.Preservation.Command.JourneyCommands.CreateJourney
{
    public class CreateJourneyCommandValidator : AbstractValidator<CreateJourneyCommand>
    {
        public CreateJourneyCommandValidator(IJourneyValidator journeyValidator)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(command => command)
                .MustAsync((command, token) => HaveUniqueJourneyTitleAsync(command.Title, token))
                .WithMessage(_ => "Journey with title already exists!");

            async Task<bool> HaveUniqueJourneyTitleAsync(string journeyTitle, CancellationToken token)
                => !await journeyValidator.ExistsWithSameTitleAsync(journeyTitle, token);
        }
    }
}
