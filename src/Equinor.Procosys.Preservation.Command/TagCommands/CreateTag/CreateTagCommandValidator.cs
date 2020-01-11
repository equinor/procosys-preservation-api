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
            RuleFor(x => x.StepId)
                .StepMustExist(journeyRepository);

            RuleForEach(tag => tag.Requirements)
                .Must(BeAnExistingRequirement)
                .WithMessage((tag, req) => $"Requirement {req.RequirementDefinitionId} for tag {tag.TagNo} don't exists");

            bool BeAnExistingRequirement(RequirementDto requirement)
                => requirementValidator.Exists(requirement.RequirementDefinitionId);
        }
    }
}
