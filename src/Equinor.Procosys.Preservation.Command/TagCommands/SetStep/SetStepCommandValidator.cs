using Equinor.Procosys.Preservation.Command.Validators;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.TagCommands.SetStep
{
    public class SetStepCommandValidator : AbstractValidator<SetStepCommand>
    {
        public SetStepCommandValidator(IJourneyRepository journeyRepository, ITagRepository tagRepository)
        {
            RuleFor(x => x.JourneyId)
                .JourneyMustExist(journeyRepository);

            RuleFor(x => x.TagId)
                .TagMustExist(tagRepository);

            RuleFor(x => x.StepId)
                .StepMustExist(journeyRepository);
        }
    }
}
