using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.ModeCommands.CreateMode
{
    public class CreateModeCommandValidator : AbstractValidator<CreateModeCommand>
    {
        public CreateModeCommandValidator()
        {
            RuleFor(x => x.Title)
                .MinimumLength(3)
                .MaximumLength(255);
        }
    }
}
