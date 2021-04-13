using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Validators;
using Equinor.ProCoSys.Preservation.Command.Validators.ModeValidators;
using FluentValidation;

namespace Equinor.ProCoSys.Preservation.Command.ModeCommands.UnvoidMode
{
    public class UnvoidModeCommandValidator : AbstractValidator<UnvoidModeCommand>
    {
        public UnvoidModeCommandValidator(
            IModeValidator modeValidator,
            IRowVersionValidator rowVersionValidator)
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(command => command)
                .MustAsync((command, token) => BeAnExistingModeAsync(command.ModeId, token))
                .WithMessage(command => $"Mode doesn't exist! Mode={command.ModeId}")
                .MustAsync((command, token) => BeAVoidedModeAsync(command.ModeId, token))
                .WithMessage(command => $"Mode is not voided! Mode={command.ModeId}")
                .Must(command => HaveAValidRowVersion(command.RowVersion))
                .WithMessage(command => $"Not a valid row version! Row version={command.RowVersion}");

            async Task<bool> BeAnExistingModeAsync(int modeId, CancellationToken token)
                => await modeValidator.ExistsAsync(modeId, token);
            async Task<bool> BeAVoidedModeAsync(int modeId, CancellationToken token)
                => await modeValidator.IsVoidedAsync(modeId, token);
            bool HaveAValidRowVersion(string rowVersion)
                => rowVersionValidator.IsValid(rowVersion);
        }
    }
}
