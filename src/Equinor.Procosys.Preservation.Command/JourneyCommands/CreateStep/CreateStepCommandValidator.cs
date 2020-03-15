using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.JourneyValidators;
using Equinor.Procosys.Preservation.Command.Validators.ModeValidators;
using Equinor.Procosys.Preservation.Command.Validators.ResponsibleValidators;
using Equinor.Procosys.Preservation.Command.Validators.StepValidators;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.CreateStep
{
    public class CreateStepCommandValidator : AbstractValidator<CreateStepCommand>
    {
        public CreateStepCommandValidator(
            IJourneyValidator journeyValidator,
            IStepValidator stepValidator,
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
                .MustAsync((command, token) => BeAnExistingResponsibleAsync(command.ResponsibleId, token))
                .WithMessage(command => $"Responsible doesn't exists! Responsible={command.ResponsibleId}")
                .MustAsync((command, token) => NotBeAVoidedResponsibleAsync(command.ResponsibleId, token))
                .WithMessage(command => $"Responsible is voided! Responsible={command.ResponsibleId}")
                .MustAsync((command, token) => HaveUniqueTitleAsync(command.JourneyId, command.Title, token))
                .WithMessage(command => $"Step with title already exists in journey! Step={command.Title}");

            async Task<bool> HaveUniqueTitleAsync(int journeyId, string title, CancellationToken token)
                => !await stepValidator.ExistsAsync(journeyId, title, token);

            async Task<bool> BeAnExistingJourney(int journeyId, CancellationToken token)
                => await journeyValidator.ExistsAsync(journeyId, token);

            async Task<bool> BeAnExistingModeAsync(int modeId, CancellationToken token)
                => await modeValidator.ExistsAsync(modeId, token);

            async Task<bool> BeAnExistingResponsibleAsync(int responsibleId, CancellationToken token)
                => await responsibleValidator.ExistsAsync(responsibleId, token);

            async Task<bool> NotBeAVoidedJourney(int journeyId, CancellationToken token)
                => !await journeyValidator.IsVoidedAsync(journeyId, token);

            async Task<bool> NotBeAVoidedModeAsync(int modeId, CancellationToken token)
                => !await modeValidator.IsVoidedAsync(modeId, token);

            async Task<bool> NotBeAVoidedResponsibleAsync(int responsibleId, CancellationToken token)
                => !await responsibleValidator.IsVoidedAsync(responsibleId, token);
        }
    }
}
