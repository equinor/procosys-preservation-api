using System.Collections.Generic;
using System.Linq;
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
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(tag => tag.TagNos)
                .Must(r => r.Any())
                .WithMessage("At least 1 TagNo must be given!")
                .Must(BeUniqueTagNos)
                .WithMessage("TagNos must be unique!");

            RuleForEach(tag => tag.TagNos)
                .Must((command, tagNo) => NotBeAnExistingTagWithinProject(tagNo, command.ProjectName))
                .WithMessage((command, tagNo) => $"Tag already exists in scope for project! Tag={tagNo} Project={command.ProjectName}");

            RuleFor(tag => tag.ProjectName)
                .Must(NotBeAClosedProject)
                .When(tag => ProjectExists(tag.ProjectName))
                .WithMessage(tag => $"Project is closed! Project={tag.ProjectName}");

            RuleFor(tag => tag.StepId)
                .Must(BeAnExistingStep)
                .WithMessage(tag => $"Step doesn't exists! Step={tag.StepId}")
                .Must(NotBeAVoidedStep)
                .WithMessage(tag => $"Step is voided! Step={tag.StepId}");

            RuleFor(tag => tag.Requirements)
                .Must(r => r.Any())
                .WithMessage(tag => $"At least 1 requirement must be given! Tag={tag.TagNos}")
                .Must(BeUniqueRequirements)
                .WithMessage(tag => $"Requirement definitions must be unique! Tag={tag.TagNos}");

            RuleForEach(tag => tag.Requirements)
                .Must(BeAnExistingRequirementDefinition)
                .WithMessage((command, req) =>
                    $"Requirement definition doesn't exists! Requirement={req.RequirementDefinitionId}")
                .Must(NotBeAVoidedRequirementDefinition)
                .WithMessage((command, req) =>
                    $"Requirement definition is voided! Requirement={req.RequirementDefinitionId}");
                        
            bool BeUniqueTagNos(IEnumerable<string> tagNos)
            {
                var lowerTagNos = tagNos.Select(t => t.ToLower()).ToList();
                return lowerTagNos.Distinct().Count() == lowerTagNos.Count;
            }

            bool NotBeAnExistingTagWithinProject(string tagNo, string projectName) => !tagValidator.Exists(tagNo, projectName);

            bool ProjectExists(string projectName) => projectValidator.Exists(projectName);

            bool NotBeAClosedProject(string projectName) => !projectValidator.IsClosed(projectName);

            bool BeAnExistingStep(int stepId) => stepValidator.Exists(stepId);

            bool NotBeAVoidedStep(int stepId) => !stepValidator.IsVoided(stepId);
                        
            bool BeUniqueRequirements(IEnumerable<Requirement> requirements)
            {
                var reqIds = requirements.Select(dto => dto.RequirementDefinitionId).ToList();
                return reqIds.Distinct().Count() == reqIds.Count;
            }

            bool BeAnExistingRequirementDefinition(Requirement requirement)
                => requirementDefinitionValidator.Exists(requirement.RequirementDefinitionId);

            bool NotBeAVoidedRequirementDefinition(Requirement requirement)
                => !requirementDefinitionValidator.IsVoided(requirement.RequirementDefinitionId);
        }
    }
}
