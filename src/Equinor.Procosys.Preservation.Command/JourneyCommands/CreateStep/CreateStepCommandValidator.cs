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
            RuleFor(x => x.JourneyId)
                .Must(BeAnExistingJourney)
                .WithMessage(x => $"Journey doesn't exists! Step={x.JourneyId}");

            RuleFor(x => x.ModeId)
                .Must(BeAnExistingMode)
                .WithMessage(x => $"Mode doesn't exists! Step={x.ModeId}");

            RuleFor(x => x.ResponsibleId)
                .Must(BeAnExistingResponsible)
                .WithMessage(x => $"Responsible doesn't exists! Step={x.ResponsibleId}");

            bool BeAnExistingJourney(int journeyId) => journeyValidator.Exists(journeyId);
            
            bool BeAnExistingMode(int modeId) => modeValidator.Exists(modeId);

            bool BeAnExistingResponsible(int responsibleId) => responsibleValidator.Exists(responsibleId);
        }
    }
}
