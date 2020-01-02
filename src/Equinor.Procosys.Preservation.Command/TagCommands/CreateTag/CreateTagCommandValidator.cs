using Equinor.Procosys.Preservation.Command.Validators;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.TagCommands.CreateTag
{
    public class CreateTagCommandValidator : AbstractValidator<CreateTagCommand>
    {
        public CreateTagCommandValidator(IJourneyRepository journeyRepository)
        {
            RuleFor(x => x.TagNo)
                .NotEmpty();

            RuleFor(x => x.ProjectNo)
                .NotEmpty();

            RuleFor(x => x.JourneyId)
                .JourneyMustExist(journeyRepository);

            RuleFor(x => x.StepId)
                .StepMustExist(journeyRepository);
        }
    }
}
