using Equinor.Procosys.Preservation.Command.Validators.Mode;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.ModeCommands.DeleteMode
{
    public class DeleteModeCommandValidator : AbstractValidator<DeleteModeCommand>
    {
        public DeleteModeCommandValidator(IModeValidator modeValidator)
        {
            RuleFor(x => x.ModeId)
                .Must(BeAnExistingMode)
                .WithMessage(x => $"Mode doesn't exists! Step={x.ModeId}");

            bool BeAnExistingMode(int modeId) => modeValidator.Exists(modeId);
        }
    }
}
