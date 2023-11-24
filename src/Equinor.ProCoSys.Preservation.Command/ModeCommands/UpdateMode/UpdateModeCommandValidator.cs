using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Validators;
using Equinor.ProCoSys.Preservation.Command.Validators.ModeValidators;
using FluentValidation;

namespace Equinor.ProCoSys.Preservation.Command.ModeCommands.UpdateMode
{
    public class UpdateModeCommandValidator : AbstractValidator<UpdateModeCommand>
    {
        public UpdateModeCommandValidator(
            IModeValidator modeValidator,
            IRowVersionValidator rowVersionValidator)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(command => command)
                .MustAsync((command, token) => BeAnExistingModeAsync(command.ModeId, token))
                .WithMessage(command => $"Mode doesn't exist! Mode={command.ModeId}")
                .MustAsync((command, token) => HaveUniqueModeTitleAsync(command.ModeId, command.Title, token))
                .WithMessage(command => $"Mode with title already exists! Mode={command.Title}")
                .MustAsync((command, token) => NotBeAVoidedModeAsync(command.ModeId, token))
                .WithMessage(command => $"Mode is voided! Mode={command.ModeId}")
                .MustAsync((command, token) => IsUniqueForSupplierAsync(command.ModeId, token))
                .WithMessage(command => $"Another mode for supplier already exists! Mode={command.Title}")
                .When(command => command.ForSupplier, ApplyConditionTo.CurrentValidator)
                .MustAsync((command, token) => NotChangeForSupplierAsync(command.ModeId, command.ForSupplier, token))
                .WithMessage(command => $"Can't change 'For supplier' when mode is used in step(s)! Mode={command.Title}")
                .WhenAsync((command, token) => UsedInAnyStepAsync(command.ModeId, token), ApplyConditionTo.CurrentValidator)
                .Must(command => HaveAValidRowVersion(command.RowVersion))
                .WithMessage(command => $"Not a valid row version! Row version={command.RowVersion}");

            async Task<bool> BeAnExistingModeAsync(int modeId, CancellationToken token)
                => await modeValidator.ExistsAsync(modeId, token);
            async Task<bool> HaveUniqueModeTitleAsync(int modeId, string modeTitle, CancellationToken token)
                => !await modeValidator.ExistsAnotherModeWithSameTitleAsync(modeId, modeTitle, token);
            async Task<bool> NotBeAVoidedModeAsync(int modeId, CancellationToken token)
                => !await modeValidator.IsVoidedAsync(modeId, token);
            async Task<bool> IsUniqueForSupplierAsync(int modeId, CancellationToken token) =>
                !await modeValidator.ExistsAnotherModeForSupplierAsync(modeId, token);
            async Task<bool> NotChangeForSupplierAsync(int modeId, bool forSupplier, CancellationToken token) =>
                await modeValidator.ExistsWithForSupplierValueAsync(modeId, forSupplier, token);
            async Task<bool> UsedInAnyStepAsync(int modeId, CancellationToken token)
                => await modeValidator.IsUsedInStepAsync(modeId, token);
            bool HaveAValidRowVersion(string rowVersion)
                => rowVersionValidator.IsValid(rowVersion);
        }
    }
}
