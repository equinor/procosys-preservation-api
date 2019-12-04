using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.ModeCommands
{
    public class CreateModeCommandValidator : AbstractValidator<CreateModeCommand>
    {
        public CreateModeCommandValidator()
        {
            RuleFor(x => x.Title).NotEmpty();
        }
    }
}
