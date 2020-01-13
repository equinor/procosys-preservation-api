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
                .WithMessage(tag => $"Tag already exists in scope for project! Tag={tag.TagNo} Project={tag.ProjectNo}");

            RuleFor(tag => tag.ProjectNo)
                .Must(NotBeAClosedProject)
                .When(tag => ProjectExists(tag.ProjectNo))
                .WithMessage(tag => $"Project is closed! Project={tag.ProjectNo}");

            RuleFor(tag => tag.StepId)
                .Must(BeAnExistingStep)
                .WithMessage(tag => $"Step don't exists! Step={tag.StepId}");

            RuleForEach(tag => tag.Requirements)
                .Must(BeAnExistingRequirementDefinition)
                .WithMessage((tag, req) => $"Requirement definition don't exists! {req.RequirementDefinitionId} ");

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
