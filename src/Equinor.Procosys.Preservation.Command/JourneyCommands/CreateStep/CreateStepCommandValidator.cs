using Equinor.Procosys.Preservation.Command.Validators;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.CreateStep
{
    public class CreateStepCommandValidator : AbstractValidator<CreateStepCommand>
    {
        public CreateStepCommandValidator(IJourneyRepository journeyRepository, IModeRepository modeRepository, IResponsibleRepository responsibleRepository)
        {
            RuleFor(x => x.JourneyId)
                .JourneyMustExist(journeyRepository);

            RuleFor(x => x.ModeId)
                .ModeMustExist(modeRepository);

            RuleFor(x => x.ResponsibleId)
                .ResponsibleMustExist(responsibleRepository);
        }
    }
}
