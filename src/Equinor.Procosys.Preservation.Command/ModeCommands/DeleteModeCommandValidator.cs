using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.ModeCommands
{
    public class DeleteModeCommandValidator : AbstractValidator<DeleteModeCommand>
    {
        public DeleteModeCommandValidator()
        {
            RuleFor(x => x.ModeId).GreaterThan(0);
        }
    }
}
