﻿using System.Threading;
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
            CascadeMode = CascadeMode.Stop;

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
            bool HaveAValidRowVersion(string rowVersion)
                => rowVersionValidator.IsValid(rowVersion);
        }
    }
}
