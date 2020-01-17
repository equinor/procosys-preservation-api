using Equinor.Procosys.Preservation.Command.Validators.Journey;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.CreateJourney
{
    public class CreateJourneyCommandValidator : AbstractValidator<CreateJourneyCommand>
    {
        public CreateJourneyCommandValidator(IJourneyValidator journeyValidator)
        {
            RuleFor(x => x.Title)
                .Must(HaveUniqueTitle)
                .WithMessage(x => $"Journey with title already exists! Journey={x.Title}");

            bool HaveUniqueTitle(string title) => !journeyValidator.Exists(title);
        }
    }
}
