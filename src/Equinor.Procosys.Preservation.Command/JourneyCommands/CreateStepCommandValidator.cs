using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands
{
    public class CreateStepCommandValidator : AbstractValidator<CreateStepCommand>
    {
        public CreateStepCommandValidator()
        {
        }
    }
}
