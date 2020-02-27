﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.ProjectValidators;
using Equinor.Procosys.Preservation.Command.Validators.RequirementDefinitionValidators;
using Equinor.Procosys.Preservation.Command.Validators.StepValidators;
using Equinor.Procosys.Preservation.Command.Validators.TagValidators;
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

            RuleFor(command => command.TagNos)
                .Must(r => r.Any())
                .WithMessage("At least 1 TagNo must be given!")
                .Must(BeUniqueTagNos)
                .WithMessage("TagNos must be unique!");

            RuleFor(command => command.Requirements)
                .Must(r => r.Any())
                .WithMessage("At least 1 requirement must be given!")
                .Must(BeUniqueRequirements)
                .WithMessage("Requirement definitions must be unique!");

            RuleForEach(command => command.TagNos)
                .MustAsync((command, tagNo, _, token) => NotBeAnExistingTagWithinProject(tagNo, command.ProjectName, token))
                .WithMessage((command, tagNo) => $"Tag already exists in scope for project! Tag={tagNo} Project={command.ProjectName}");

            RuleFor(command => command)
                .MustAsync((command, token) => NotBeAnExistingAndClosedProjectAsync(command.ProjectName, token))
                .WithMessage(command => $"Project is closed! Project={command.ProjectName}")
                .MustAsync((command, token) => BeAnExistingStepAsync(command.StepId, token))
                .WithMessage(command => $"Step doesn't exists! Step={command.StepId}")
                .MustAsync((command, token) => NotBeAVoidedStepAsync(command.StepId, token))
                .WithMessage(command => $"Step is voided! Step={command.StepId}");

            RuleForEach(tag => tag.Requirements)
                .MustAsync((_, req, __, token) => BeAnExistingRequirementDefinitionAsync(req, token))
                .WithMessage((_, req) =>
                    $"Requirement definition doesn't exists! Requirement={req.RequirementDefinitionId}")
                .MustAsync((_, req, __, token) => NotBeAVoidedRequirementDefinitionAsync(req, token))
                .WithMessage((_, req) =>
                    $"Requirement definition is voided! Requirement={req.RequirementDefinitionId}");
                        
            bool BeUniqueTagNos(IEnumerable<string> tagNos)
            {
                var lowerTagNos = tagNos.Select(t => t.ToLower()).ToList();
                return lowerTagNos.Distinct().Count() == lowerTagNos.Count;
            }
                        
            bool BeUniqueRequirements(IEnumerable<Requirement> requirements)
            {
                var reqIds = requirements.Select(dto => dto.RequirementDefinitionId).ToList();
                return reqIds.Distinct().Count() == reqIds.Count;
            }

            async Task<bool> NotBeAnExistingTagWithinProject(string tagNo, string projectName, CancellationToken token) =>
                !await tagValidator.ExistsAsync(tagNo, projectName, token);

            async Task<bool> NotBeAnExistingAndClosedProjectAsync(string projectName, CancellationToken token)
                => !await projectValidator.IsExistingAndClosedAsync(projectName, token);

            async Task<bool> BeAnExistingStepAsync(int stepId, CancellationToken token)
                => await stepValidator.ExistsAsync(stepId, token);

            async Task<bool> NotBeAVoidedStepAsync(int stepId, CancellationToken token)
                => !await stepValidator.IsVoidedAsync(stepId, token);

            async Task<bool> BeAnExistingRequirementDefinitionAsync(Requirement requirement, CancellationToken token)
                => await requirementDefinitionValidator.ExistsAsync(requirement.RequirementDefinitionId, token);

            async Task<bool> NotBeAVoidedRequirementDefinitionAsync(Requirement requirement, CancellationToken token)
                => !await requirementDefinitionValidator.IsVoidedAsync(requirement.RequirementDefinitionId, token);
        }
    }
}
