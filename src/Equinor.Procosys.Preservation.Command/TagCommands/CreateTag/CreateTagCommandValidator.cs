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
                .Must(NotBeAnExistingTagWithinProject)
                .WithMessage(tag => $"Tag already exists in scope for project! Tag={tag.TagNo} Project={tag.ProjectName}");

            RuleFor(tag => tag.ProjectName)
                .Must(NotBeAClosedProject)
                .When(tag => ProjectExists(tag.ProjectName))
                .WithMessage(tag => $"Project is closed! Project={tag.ProjectName}");

            RuleFor(tag => tag.StepId)
                .Must(BeAnExistingStep)
                .WithMessage(tag => $"Step doesn't exists! Step={tag.StepId}");
            
            RuleFor(x => x.StepId)
                .Must(NotBeAVoidedStep)
                .WithMessage(x => $"Step is voided! Step={x.StepId}");

            RuleForEach(tag => tag.Requirements)
                .Must(BeAnExistingRequirementDefinition)
                .WithMessage((tag, req) => $"Requirement definition doesn't exists! Requirement={req.RequirementDefinitionId} ");

            RuleForEach(tag => tag.Requirements)
                .Must(NotBeAVoidedRequirementDefinition)
                .WithMessage((tag, req) => $"Requirement definition is voided! Requirement={req.RequirementDefinitionId} ");

            bool NotBeAnExistingTagWithinProject(CreateTagCommand tag) => !tagValidator.Exists(tag.TagNo, tag.ProjectName);

            bool BeAnExistingStep(int stepId) => stepValidator.Exists(stepId);
            
            bool NotBeAVoidedStep(int stepId) => !stepValidator.IsVoided(stepId);

            bool ProjectExists(string projectName) => projectValidator.Exists(projectName);

            bool NotBeAClosedProject(string projectName) => !projectValidator.IsClosed(projectName);

            bool BeAnExistingRequirementDefinition(Requirement requirement)
                => requirementDefinitionValidator.Exists(requirement.RequirementDefinitionId);

            bool NotBeAVoidedRequirementDefinition(Requirement requirement)
                => !requirementDefinitionValidator.IsVoided(requirement.RequirementDefinitionId);
        }
    }
}
