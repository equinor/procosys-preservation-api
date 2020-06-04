using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.ModeValidators;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.ModeCommands.UpdateMode
{
    public class UpdateModeCommandValidator : AbstractValidator<UpdateModeCommand>
    {
        public UpdateModeCommandValidator(IModeValidator modeValidator)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(command => command)
                .MustAsync((command, token) => BeAnExistingModeAsync(command.ModeId, token))
                .WithMessage(command => $"Mode doesn't exists! Mode={command.ModeId}")
                .MustAsync((command, token) => HaveUniqueModeTitleAsync(command.ModeId, command.Title, token))
                .WithMessage(command => $"Mode with title already exists! Mode={command.Title}")
                .MustAsync((command, token) => NotBeAVoidedModeAsync(command.ModeId, token))
                .WithMessage(command => $"Mode is voided! Mode={command.ModeId}")
                .MustAsync((command, token) => IsUniqueForSupplierAsync(command.ModeId, command.ForSupplier, token))
                .WithMessage(command => $"Another mode for supplier already exists! Mode={command.Title}");

            async Task<bool> BeAnExistingModeAsync(int modeId, CancellationToken token)
                => await modeValidator.ExistsAsync(modeId, token);
            async Task<bool> HaveUniqueModeTitleAsync(int modeId, string modeTitle, CancellationToken token)
                => !await modeValidator.ExistsAnotherModeWithSameTitleAsync(modeId, modeTitle, token);
            async Task<bool> NotBeAVoidedModeAsync(int modeId, CancellationToken token)
                => !await modeValidator.IsVoidedAsync(modeId, token);
            async Task<bool> IsUniqueForSupplierAsync(int modeId, bool forSupplier, CancellationToken token) =>
                !await modeValidator.ExistsAnotherModeIdForSupplierAsync(modeId, forSupplier, token);
        }
    }
}
