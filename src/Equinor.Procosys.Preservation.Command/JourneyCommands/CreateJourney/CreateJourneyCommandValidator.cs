using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.CreateJourney
{
    public class CreateJourneyCommandValidator : AbstractValidator<CreateJourneyCommand>
    {
        public CreateJourneyCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty();
        }
    }
}
