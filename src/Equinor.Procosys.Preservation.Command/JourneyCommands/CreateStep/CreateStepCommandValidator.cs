using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.JourneyValidators;
using Equinor.Procosys.Preservation.Command.Validators.ModeValidators;
using Equinor.Procosys.Preservation.Command.Validators.ResponsibleValidators;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.CreateStep
{
    public class CreateStepCommandValidator : AbstractValidator<CreateStepCommand>
    {
        public CreateStepCommandValidator(
            IJourneyValidator journeyValidator,
            IModeValidator modeValidator,
            IResponsibleValidator responsibleValidator)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(command => command)
                .MustAsync((command, token) => BeAnExistingJourney(command.JourneyId, token))
                .WithMessage(command => $"Journey doesn't exists! Journey={command.JourneyId}")
                .MustAsync((command, token) => NotBeAVoidedJourney(command.JourneyId, token))
                .WithMessage(command => $"Journey is voided! Journey={command.JourneyId}")
                .MustAsync((command, token) => BeAnExistingModeAsync(command.ModeId, token))
                .WithMessage(command => $"Mode doesn't exists! Mode={command.ModeId}")
                .MustAsync((command, token) => NotBeAVoidedModeAsync(command.ModeId, token))
                .WithMessage(command => $"Mode is voided! Mode={command.ModeId}")
                .MustAsync((command, token) => NotBeAnExistingAndVoidedResponsibleAsync(command.ResponsibleCode, token))
                .WithMessage(command => $"Responsible is voided! Responsible={command.ResponsibleCode}")
                .MustAsync((command, token) => HaveUniqueStepTitleAsync(command.JourneyId, command.Title, token))
                .WithMessage(command => $"Step with title already exists in journey! Step={command.Title}")
                .MustAsync((command, token) => BeFirstStepWhenSupplierModeAsync(command.JourneyId, command.ModeId, token))
                .WithMessage(command => $"Supplier step can only be chosen as the first step! Step={command.Title}")
                .Must(command => NotSetBothTransferOnRfocSignAndTransferOnRfccSign(command.TransferOnRfccSign, command.TransferOnRfocSign))
                .WithMessage(command => $"Both 'Transfer on RFCC signing' and 'Transfer on RFOC signing' can not be set in same step! Step={command.Title}")
                .MustAsync((command, token) => NotBeManyTransferOnRfccSignInSameJourneyAsync(command.JourneyId, command.TransferOnRfccSign, token))
                .WithMessage(command => "'Transfer on RFCC signing' can not be set on multiple steps in a journey!")
                .MustAsync((command, token) => NotBeManyTransferOnRfocSignInSameJourneyAsync(command.JourneyId, command.TransferOnRfocSign, token))
                .WithMessage(command => "'Transfer on RFOC signing' can not be set on multiple steps in a journey!");

            async Task<bool> HaveUniqueStepTitleAsync(int journeyId, string stepTitle, CancellationToken token)
                => !await journeyValidator.AnyStepExistsWithSameTitleAsync(journeyId, stepTitle, token);

            async Task<bool> BeAnExistingJourney(int journeyId, CancellationToken token)
                => await journeyValidator.ExistsAsync(journeyId, token);

            async Task<bool> BeAnExistingModeAsync(int modeId, CancellationToken token)
                => await modeValidator.ExistsAsync(modeId, token);

            async Task<bool> NotBeAnExistingAndVoidedResponsibleAsync(string responsibleCode, CancellationToken token)
                => !await responsibleValidator.ExistsAndIsVoidedAsync(responsibleCode, token);

            async Task<bool> NotBeAVoidedJourney(int journeyId, CancellationToken token)
                => !await journeyValidator.IsVoidedAsync(journeyId, token);

            async Task<bool> NotBeAVoidedModeAsync(int modeId, CancellationToken token)
                => !await modeValidator.IsVoidedAsync(modeId, token);

            async Task<bool> BeFirstStepWhenSupplierModeAsync(int journeyId, int modeId, CancellationToken token)
            {
                var isFirstStep = !await journeyValidator.HasAnyStepsAsync(journeyId, token);
                return isFirstStep || !await modeValidator.IsForSupplierAsync(modeId, token);
            }

            bool NotSetBothTransferOnRfocSignAndTransferOnRfccSign(bool transferOnRfccSign, bool transferOnRfocSign)
                => !transferOnRfccSign || !transferOnRfocSign;
            
            async Task<bool> NotBeManyTransferOnRfccSignInSameJourneyAsync(int journeyId, bool transferOnRfccSign, CancellationToken token)
                => !transferOnRfccSign && !await journeyValidator.HasAnyStepWithTransferOnRfccSignAsync(journeyId, token);
            
            async Task<bool> NotBeManyTransferOnRfocSignInSameJourneyAsync(int journeyId, bool transferOnRfocSign, CancellationToken token)
                => !transferOnRfocSign && !await journeyValidator.HasAnyStepWithTransferOnRfocSignAsync(journeyId, token);
        }
    }
}
