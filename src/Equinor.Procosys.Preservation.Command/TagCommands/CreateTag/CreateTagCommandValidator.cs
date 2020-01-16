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
                .WithMessage(tag => $"Tag already exists in scope for project! Tag={tag.TagNo} Project={tag.ProjectNo}");

            RuleFor(tag => tag.ProjectNo)
                .Must(NotBeAClosedProject)
                .When(tag => ProjectExists(tag.ProjectNo))
                .WithMessage(tag => $"Project is closed! Project={tag.ProjectNo}");

            RuleFor(tag => tag.StepId)
                .Must(BeAnExistingStep)
                .WithMessage(tag => $"Step doesn't exists! Step={tag.StepId}");
            
            RuleFor(x => x.StepId)
                .Must(NotBeAVoidedStep)
                .WithMessage(x => $"Step is voided! Step={x.StepId}");

            RuleForEach(tag => tag.Requirements)
                .Must(BeAnExistingRequirementDefinition)
                .WithMessage((tag, req) => $"Requirement definition doesn't exists! {req.RequirementDefinitionId} ");

            RuleForEach(tag => tag.Requirements)
                .Must(NotBeAVoidedRequirementDefinition)
                .WithMessage((tag, req) => $"Requirement definition is voided! {req.RequirementDefinitionId} ");

            bool NotBeAnExistingTagWithinProject(CreateTagCommand tag) => !tagValidator.Exists(tag.TagNo, tag.ProjectNo);

            bool BeAnExistingStep(int stepId) => stepValidator.Exists(stepId);
            
            bool NotBeAVoidedStep(int stepId) => !stepValidator.IsVoided(stepId);

            bool ProjectExists(string projectNo) => projectValidator.Exists(projectNo);

            bool NotBeAClosedProject(string projectNo) => !projectValidator.IsClosed(projectNo);

            bool BeAnExistingRequirementDefinition(Requirement requirement)
                => requirementDefinitionValidator.Exists(requirement.RequirementDefinitionId);

            bool NotBeAVoidedRequirementDefinition(Requirement requirement)
                => !requirementDefinitionValidator.IsVoided(requirement.RequirementDefinitionId);
        }
    }
}
