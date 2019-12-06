using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands
{
    public class CreateStepCommandValidator : AbstractValidator<CreateStepCommand>
    {
        public CreateStepCommandValidator()
        {
            RuleFor(x => x.JourneyId).GreaterThan(0);
            RuleFor(x => x.ModeId).GreaterThan(0);
        }
    }
}
