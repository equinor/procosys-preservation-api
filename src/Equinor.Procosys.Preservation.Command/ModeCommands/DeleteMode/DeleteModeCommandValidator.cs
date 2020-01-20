using Equinor.Procosys.Preservation.Command.Validators.Mode;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.ModeCommands.DeleteMode
{
    public class DeleteModeCommandValidator : AbstractValidator<DeleteModeCommand>
    {
        public DeleteModeCommandValidator(IModeValidator modeValidator)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            
            RuleFor(x => x.ModeId)
                .Must(BeAnExistingMode)
                .WithMessage(x => $"Mode doesn't exists! Mode={x.ModeId}")
                .Must(BeAVoidedMode)
                .WithMessage(x => $"Mode is not voided! Mode={x.ModeId}")
                .Must(NotBeUsedInAnyStep)
                .WithMessage(x => $"Mode is used in step(s)! Mode={x.ModeId}");

            bool BeAnExistingMode(int modeId) => modeValidator.Exists(modeId);

            bool BeAVoidedMode(int modeId) => modeValidator.IsVoided(modeId);
            
            bool NotBeUsedInAnyStep(int modeId) => !modeValidator.IsUsedInStep(modeId);
        }
    }
}
