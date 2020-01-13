using Equinor.Procosys.Preservation.Command.Validators;
using Equinor.Procosys.Preservation.Command.Validators.Project;
using Equinor.Procosys.Preservation.Command.Validators.RequirementDefinition;
using Equinor.Procosys.Preservation.Command.Validators.Tag;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.TagCommands.CreateTag
{
    public class CreateTagCommandValidator : AbstractValidator<CreateTagCommand>
    {
        public CreateTagCommandValidator(
            IJourneyRepository journeyRepository,
            ITagValidator tagValidator,
            IProjectValidator projectValidator,
            IRequirementDefinitionValidator requirementDefinitionValidator)
        {
            RuleFor(tag => tag)
                .Must(TagNotAlreadyExists)
                .WithMessage(tag => $"Tag {tag.TagNo} for project {tag.ProjectNo} already exists in scope");

            RuleFor(tag => tag.ProjectNo)
                .Must(NotBeAClosedProject)
                .When(tag => ProjectExists(tag.ProjectNo))
                .WithMessage(tag => $"Project {tag.ProjectNo} is closed");

            RuleFor(x => x.StepId)
                .StepMustExist(journeyRepository);

            RuleForEach(tag => tag.Requirements)
                .Must(BeAnExistingRequirementDefinition)
                .WithMessage((tag, req) => $"Requirement definition {req.RequirementDefinitionId} for tag {tag.TagNo} don't exists");

            bool ProjectExists(string projectNo) => projectValidator.Exists(projectNo);

            bool NotBeAClosedProject(string projectNo) => !projectValidator.IsClosed(projectNo);

            bool TagNotAlreadyExists(CreateTagCommand tag)
                => !tagValidator.Exists(tag.TagNo, tag.ProjectNo);

            bool BeAnExistingRequirementDefinition(RequirementDto requirement)
                => requirementDefinitionValidator.Exists(requirement.RequirementDefinitionId);
        }
    }
}
