using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.TagCommands
{
    public class CreateTagCommandValidator : AbstractValidator<CreateTagCommand>
    {
        public CreateTagCommandValidator()
        {
            RuleFor(x => x.ProjectNo).NotEmpty();
            RuleFor(x => x.JourneyId).GreaterThan(0);
            RuleFor(x => x.StepId).GreaterThan(0);
            RuleFor(x => x.TagNo).NotEmpty();
        }
    }
}
