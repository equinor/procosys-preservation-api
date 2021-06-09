using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Validators.JourneyValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.ModeValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.ResponsibleValidators;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using FluentValidation;

namespace Equinor.ProCoSys.Preservation.Command.JourneyCommands.CreateStep
{
    public class CreateStepCommandValidator : AbstractValidator<CreateStepCommand>
    {
        public CreateStepCommandValidator(
            IJourneyValidator journeyValidator,
            IModeValidator modeValidator,
            IResponsibleValidator responsibleValidator)
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(command => command)
                .MustAsync((command, token) => BeAnExistingJourney(command.JourneyId, token))
                .WithMessage(command => $"Journey doesn't exist! Journey={command.JourneyId}")
                .MustAsync((command, token) => NotBeAVoidedJourney(command.JourneyId, token))
                .WithMessage(command => $"Journey is voided! Journey={command.JourneyId}")
                .MustAsync((command, token) => BeAnExistingModeAsync(command.ModeId, token))
                .WithMessage(command => $"Mode doesn't exist! Mode={command.ModeId}")
                .MustAsync((command, token) => NotBeAVoidedModeAsync(command.ModeId, token))
                .WithMessage(command => $"Mode is voided! Mode={command.ModeId}")
                .MustAsync((command, token) => NotBeAnExistingAndVoidedResponsibleAsync(command.ResponsibleCode, token))
                .WithMessage(command => $"Responsible is voided! Responsible={command.ResponsibleCode}")
                .MustAsync((command, token) => HaveUniqueStepTitleAsync(command.JourneyId, command.Title, token))
                .WithMessage(command => $"Step with title already exists in journey! Step={command.Title}")
                .MustAsync((command, token) => NotBeAnyStepWithSameAutoTransferMethodInJourneyAsync(command.JourneyId, command.AutoTransferMethod, token))
                .WithMessage(command => $"Same auto transfer method can not be set on multiple steps in a journey! Method={command.AutoTransferMethod}");

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
            
            async Task<bool> NotBeAnyStepWithSameAutoTransferMethodInJourneyAsync(int journeyId, AutoTransferMethod autoTransferMethod, CancellationToken token)
                => autoTransferMethod == AutoTransferMethod.None || !await journeyValidator.HasAnyStepWithAutoTransferMethodAsync(journeyId, autoTransferMethod, token);
        }
    }
}
