using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.TagCommands
{
    public class SetStepCommandValidator : AbstractValidator<SetStepCommand>
    {
        public SetStepCommandValidator()
        {
        }
    }
}
