using Equinor.Procosys.Preservation.Command.Validators.Project;
using Equinor.Procosys.Preservation.Command.Validators.RequirementDefinition;
using Equinor.Procosys.Preservation.Command.Validators.Step;
using Equinor.Procosys.Preservation.Command.Validators.Tag;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.TagCommands.CreateTag
{
    public class CreateTagCommandValidator : AbstractValidator<CreateTagCommand>
    {
        public CreateTagCommandValidator(
            ITagValidator tagValidator,
            IStepValidator stepValidator,
            IProjectValidator projectValidator,
            IRequirementDefinitionValidator requirementDefinitionValidator)
        {
            RuleFor(tag => tag)
                .Must(NotBeAnExistingTag)
                .WithMessage(tag => $"Tag {tag.TagNo} for project {tag.ProjectNo} already exists in scope");

            RuleFor(tag => tag.ProjectNo)
                .Must(NotBeAClosedProject)
                .When(tag => ProjectExists(tag.ProjectNo))
                .WithMessage(tag => $"Project {tag.ProjectNo} is closed for tag {tag.TagNo}");

            RuleFor(tag => tag.StepId)
                .Must(BeAnExistingStep)
                .WithMessage(tag => $"Step {tag.StepId} for tag {tag.TagNo} don't exists");

            RuleForEach(tag => tag.Requirements)
                .Must(BeAnExistingRequirementDefinition)
                .WithMessage((tag, req) => $"Requirement definition {req.RequirementDefinitionId} for tag {tag.TagNo} don't exists");

            bool NotBeAnExistingTag(CreateTagCommand tag)
                => !tagValidator.Exists(tag.TagNo, tag.ProjectNo);

            bool BeAnExistingStep(int stepId)
                => stepValidator.Exists(stepId);

            bool ProjectExists(string projectNo) => projectValidator.Exists(projectNo);

            bool NotBeAClosedProject(string projectNo) => !projectValidator.IsClosed(projectNo);

            bool BeAnExistingRequirementDefinition(RequirementDto requirement)
                => requirementDefinitionValidator.Exists(requirement.RequirementDefinitionId);
        }
    }
}
