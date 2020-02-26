using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.ProjectValidators;
using Equinor.Procosys.Preservation.Command.Validators.RequirementDefinition;
using Equinor.Procosys.Preservation.Command.Validators.Step;
using Equinor.Procosys.Preservation.Command.Validators.TagValidators;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.TagCommands.CreateAreaTag
{
    public class CreateAreaTagCommandValidator : AbstractValidator<CreateAreaTagCommand>
    {
        public CreateAreaTagCommandValidator(
            ITagValidator tagValidator,
            IStepValidator stepValidator,
            IProjectValidator projectValidator,
            IRequirementDefinitionValidator requirementDefinitionValidator)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(command => command)
                .MustAsync((command, token) => NotBeAnExistingAndClosedProjectAsync(command.ProjectName, token))
                .WithMessage(command => $"Project is closed! Project={command.ProjectName}")
                .MustAsync((command, token) => NotBeAnExistingTagWithinProject(command.GetTagNo(), command.ProjectName, token))
                .WithMessage(command => $"Tag already exists in scope for project! Tag={command.GetTagNo()} Project={command.ProjectName}");

            RuleFor(tag => tag.StepId)
                .Must(BeAnExistingStep)
                .WithMessage(tag => $"Step doesn't exists! Step={tag.StepId}")
                .Must(NotBeAVoidedStep)
                .WithMessage(tag => $"Step is voided! Step={tag.StepId}");

            RuleFor(tag => tag.Requirements)
                .Must(r => r.Any())
                .WithMessage(tag => $"At least 1 requirement must be given! Tag={tag.GetTagNo()}")
                .Must(BeUniqueRequirements)
                .WithMessage(tag => $"Requirement definitions must be unique! Tag={tag.GetTagNo()}");

            RuleForEach(tag => tag.Requirements)
                .Must(BeAnExistingRequirementDefinition)
                .WithMessage((command, req) =>
                    $"Requirement definition doesn't exists! Requirement={req.RequirementDefinitionId}")
                .Must(NotBeAVoidedRequirementDefinition)
                .WithMessage((command, req) =>
                    $"Requirement definition is voided! Requirement={req.RequirementDefinitionId}");

            async Task<bool> NotBeAnExistingAndClosedProjectAsync(string projectName, CancellationToken token)
                => !await projectValidator.IsExistingAndClosedAsync(projectName, token);

            async Task<bool> NotBeAnExistingTagWithinProject(string tagNo, string projectName, CancellationToken token)
                => !await tagValidator.ExistsAsync(tagNo, projectName, token);

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
