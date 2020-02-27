using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.ModeValidators;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.ModeCommands.DeleteMode
{
    public class DeleteModeCommandValidator : AbstractValidator<DeleteModeCommand>
    {
        public DeleteModeCommandValidator(IModeValidator modeValidator)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            
            RuleFor(command => command)
                .MustAsync((command, token) => BeAnExistingMode(command.ModeId, token))
                .WithMessage(command => $"Mode doesn't exists! Mode={command.ModeId}")
                .MustAsync((command, token) => BeAVoidedMode(command.ModeId, token))
                .WithMessage(command => $"Mode is not voided! Mode={command.ModeId}")
                .MustAsync((command, token) => NotBeUsedInAnyStep(command.ModeId, token))
                .WithMessage(command => $"Mode is used in step(s)! Mode={command.ModeId}");

            async Task<bool> BeAnExistingMode(int modeId, CancellationToken token)
                => await modeValidator.ExistsAsync(modeId, token);

            async Task<bool> BeAVoidedMode(int modeId, CancellationToken token)
                => await modeValidator.IsVoidedAsync(modeId, token);

            async Task<bool> NotBeUsedInAnyStep(int modeId, CancellationToken token)
                => !await modeValidator.IsUsedInStepAsync(modeId, token);
        }
    }
}
