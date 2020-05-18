using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.ModeValidators;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.ModeCommands.UnvoidMode
{
    public class UnvoidModeCommandValidator : AbstractValidator<UnvoidModeCommand>
    {
        public UnvoidModeCommandValidator(IModeValidator modeValidator)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(command => command)
                .MustAsync((command, token) => BeAnExistingModeAsync(command.ModeId, token))
                .WithMessage(command => $"Mode doesn't exist! Mode={command.ModeId}")
                .MustAsync((command, token) => BeAVoidedModeAsync(command.ModeId, token))
                .WithMessage(command => $"Mode is not voided! Mode={command.ModeId}");

            async Task<bool> BeAnExistingModeAsync(int modeId, CancellationToken token)
                => await modeValidator.ExistsAsync(modeId, token);
            async Task<bool> BeAVoidedModeAsync(int modeId, CancellationToken token)
                => await modeValidator.IsVoidedAsync(modeId, token);
        }
    }
}
