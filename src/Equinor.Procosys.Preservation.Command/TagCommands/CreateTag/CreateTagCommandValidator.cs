using Equinor.Procosys.Preservation.Command.Validators;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.TagCommands.CreateTag
{
    public class CreateTagCommandValidator : AbstractValidator<CreateTagCommand>
    {
        public CreateTagCommandValidator(IJourneyRepository journeyRepository,
            IRequirementValidator requirementValidator)
        {
            RuleFor(x => x.JourneyId)
                .JourneyMustExist(journeyRepository);

            RuleFor(x => x.StepId)
                .StepMustExist(journeyRepository);

            RuleForEach(tag => tag.RequirementDefinitionIds)
                .Must(BeAnExistingRequirement)
                .WithMessage("Requirement don't exists");

            bool BeAnExistingRequirement(int requirementId) => requirementValidator.Exists(requirementId);
        }
    }
}
