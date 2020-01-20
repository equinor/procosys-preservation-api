using Equinor.Procosys.Preservation.Command.Validators.Journey;
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

            RuleFor(x => x.JourneyId)
                .Must(BeAnExistingJourney)
                .WithMessage(x => $"Journey doesn't exists! Journey={x.JourneyId}")
                .Must(NotBeAVoidedJourney)
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

            bool BeAnExistingJourney(int journeyId) => journeyValidator.Exists(journeyId);
            
            bool BeAnExistingMode(int modeId) => modeValidator.Exists(modeId);

            bool BeAnExistingResponsible(int responsibleId) => responsibleValidator.Exists(responsibleId);

            bool NotBeAVoidedJourney(int journeyId) => !journeyValidator.IsVoided(journeyId);
            
            bool NotBeAVoidedMode(int modeId) => !modeValidator.IsVoided(modeId);

            bool NotBeAVoidedResponsible(int responsibleId) => !responsibleValidator.IsVoided(responsibleId);
        }
    }
}
