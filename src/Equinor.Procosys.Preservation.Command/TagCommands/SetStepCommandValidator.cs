using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.TagCommands
{
    public class SetStepCommandValidator : AbstractValidator<SetStepCommand>
    {
        public SetStepCommandValidator()
        {
            RuleFor(x => x.StepId).GreaterThan(0);
            RuleFor(x => x.TagId).GreaterThan(0);
        }
    }
}
