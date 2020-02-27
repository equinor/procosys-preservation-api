using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.JourneyValidators;
using Equinor.Procosys.Preservation.Command.Validators.Mode;
using Equinor.Procosys.Preservation.Command.Validators.Responsible;
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
                .WithMessage(x => $"Journey doesn't exists! Journey={x.JourneyId}")
                .MustAsync((command, token) => NotBeAVoidedJourney(command.JourneyId, token))
                .WithMessage(x => $"Journey is voided! Journey={x.JourneyId}");

            RuleFor(x => x.ModeId)
                .Must(BeAnExistingMode)
                .WithMessage(x => $"Mode doesn't exists! Mode={x.ModeId}")
                .Must(NotBeAVoidedMode)
                .WithMessage(x => $"Mode is voided! Mode={x.ModeId}");

            RuleFor(x => x.ResponsibleId)
                .Must(BeAnExistingResponsible)
                .WithMessage(x => $"Responsible doesn't exists! Responsible={x.ResponsibleId}")
                .Must(NotBeAVoidedResponsible)
                .WithMessage(x => $"Responsible is voided! Responsible={x.ResponsibleId}");

            async Task<bool> BeAnExistingJourney(int journeyId, CancellationToken token)
                => await journeyValidator.ExistsAsync(journeyId, token);
            
            bool BeAnExistingMode(int modeId) => modeValidator.Exists(modeId);

            bool BeAnExistingResponsible(int responsibleId) => responsibleValidator.Exists(responsibleId);

            async Task<bool> NotBeAVoidedJourney(int journeyId, CancellationToken token)
                => !await journeyValidator.IsVoidedAsync(journeyId, token);
            
            bool NotBeAVoidedMode(int modeId) => !modeValidator.IsVoided(modeId);

            bool NotBeAVoidedResponsible(int responsibleId) => !responsibleValidator.IsVoided(responsibleId);
        }
    }
}
