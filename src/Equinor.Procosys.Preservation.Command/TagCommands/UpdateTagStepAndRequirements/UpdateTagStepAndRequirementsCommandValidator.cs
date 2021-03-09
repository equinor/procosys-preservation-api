﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators;
using Equinor.Procosys.Preservation.Command.Validators.ProjectValidators;
using Equinor.Procosys.Preservation.Command.Validators.RequirementDefinitionValidators;
using Equinor.Procosys.Preservation.Command.Validators.StepValidators;
using Equinor.Procosys.Preservation.Command.Validators.TagValidators;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.TagCommands.UpdateTagStepAndRequirements
{
    public class UpdateTagStepAndRequirementsCommandValidator : AbstractValidator<UpdateTagStepAndRequirementsCommand>
    {
        public UpdateTagStepAndRequirementsCommandValidator(
             IProjectValidator projectValidator,
             ITagValidator tagValidator,
             IStepValidator stepValidator,
             IRequirementDefinitionValidator requirementDefinitionValidator,
             IRowVersionValidator rowVersionValidator)
        {
            CascadeMode = CascadeMode.Stop;

            WhenAsync((command, token) => IsASupplierStepAsync(command.StepId, token), () =>
            {
                RuleFor(command => command)
                    .MustAsync((_, command, token) =>
                        RequirementsMustBeUniqueAfterUpdateAsync(
                            command.TagId,
                            command.NewRequirements.Select(r => r.RequirementDefinitionId).ToList(),
                            token))
                    .WithMessage(command => "Requirements must be unique!")
                    .MustAsync((_, command, token) =>
                        RequirementUsageIsForAllJourneysAsync(
                            command.TagId,
                            command.UpdateRequirements.Where(u => !u.IsVoided).Select(u => u.TagRequirementId).ToList(),
                            command.UpdateRequirements.Where(u => u.IsVoided).Select(u => u.TagRequirementId).ToList(),
                            command.NewRequirements.Select(r => r.RequirementDefinitionId).ToList(),
                            token))
                    .WithMessage(command => "Requirements must include requirements to be used both for supplier and other than suppliers!");
            }).Otherwise(() =>
            {
                RuleFor(command => command)
                    .MustAsync((command, token) => NotBeAPoAreaTagAsync(command.TagId, token))
                    .WithMessage(command => $"Step for a {TagType.PoArea.GetTagNoPrefix()} tag need to be for supplier!")
                    .MustAsync((_, command, token) =>
                        RequirementsMustBeUniqueAfterUpdateAsync(
                            command.TagId,
                            command.NewRequirements.Select(r => r.RequirementDefinitionId).ToList(),
                            token))
                    .WithMessage(command => "Requirements must be unique!")
                    .MustAsync((_, command, token) =>
                        RequirementUsageIsForJourneysWithoutSupplierAsync(
                            command.TagId,
                            command.UpdateRequirements.Where(u => !u.IsVoided).Select(u => u.TagRequirementId).ToList(),
                            command.UpdateRequirements.Where(u => u.IsVoided).Select(u => u.TagRequirementId).ToList(),
                            command.NewRequirements.Select(r => r.RequirementDefinitionId).ToList(),
                            token))
                    .WithMessage(command => "Requirements must include requirements to be used for other than suppliers!");
            });

            WhenAsync((command, token) => IsAnAreaTagAsync(command.TagId, token), () =>
            {
                RuleFor(command => command)
                    .Must(command => !string.IsNullOrEmpty(command.Description))
                    .WithMessage(command => "Description can not be blank!");
            }).Otherwise(() =>
            {
                RuleFor(command => command)
                    .MustAsync((command, token)
                        => NotChangeDescriptionAsync(command.TagId, command.Description, token))
                    .WithMessage(command => "Tag must be an area tag to update description!");
            });

            RuleFor(command => command)
                .MustAsync((command, token) => NotBeAClosedProjectForTagAsync(command.TagId, token))
                .WithMessage(command => $"Project for tag is closed! Tag={command.TagId}")
                .MustAsync((command, token) => BeAnExistingTagAsync(command.TagId, token))
                .WithMessage(command => $"Tag doesn't exist! Tag={command.TagId}")
                .MustAsync((command, token) => NotBeAVoidedTagAsync(command.TagId, token))
                .WithMessage(command => $"Tag is voided! Tag={command.TagId}")
                .MustAsync((command, token) => NotChangedToAVoidedStepAsync(command.TagId, command.StepId, token))
                .WithMessage(command => $"Step is voided! Step={command.StepId}")
                .Must(command => HaveAValidRowVersion(command.RowVersion))
                .WithMessage(command => $"Not a valid row version! Row version={command.RowVersion}");

            RuleForEach(command => command.UpdateRequirements)
                .MustAsync((command, req, __, token) => BeAnExistingTagRequirementAsync(command.TagId, req.TagRequirementId, token))
                .WithMessage((_, req) => $"Requirement doesn't exist! Requirement={req.TagRequirementId}");

            RuleForEach(command => command.DeleteRequirements)
                .MustAsync((command, req, __, token) => BeAnExistingTagRequirementAsync(command.TagId, req.TagRequirementId, token))
                .WithMessage((_, req) => $"Requirement doesn't exist! Requirement={req.TagRequirementId}")
                .MustAsync((command, req, __, token) => BeAVoidedTagRequirementAsync(
                    command.TagId,
                    req.TagRequirementId,
                    command.UpdateRequirements.Where(u => u.IsVoided).Select(u => u.TagRequirementId).ToList(),
                    token))
                .WithMessage((_, req) => $"Requirement is not voided! Requirement={req.TagRequirementId}");
            
            RuleForEach(command => command.NewRequirements)
                .MustAsync((_, req, __, token) => BeAnExistingRequirementDefinitionAsync(req.RequirementDefinitionId, token))
                .WithMessage((_, req) =>
                    $"Requirement definition doesn't exist! Requirement definition={req.RequirementDefinitionId}")
                .MustAsync((_, req, __, token) => NotBeAVoidedRequirementDefinitionAsync(req.RequirementDefinitionId, token))
                .WithMessage((_, req) =>
                    $"Requirement definition is voided! Requirement definition={req.RequirementDefinitionId}");

            async Task<bool> RequirementsMustBeUniqueAfterUpdateAsync(
                int tagId,
                List<int> requirementDefinitionIdsToBeAdded,
                CancellationToken token)
                => requirementDefinitionIdsToBeAdded.Count == 0 || 
                   await tagValidator.AllRequirementsWillBeUniqueAsync(tagId, requirementDefinitionIdsToBeAdded, token);
            
            async Task<bool> RequirementUsageIsForAllJourneysAsync(
                int tagId, 
                List<int> tagRequirementIdsToBeUnvoided,
                List<int> tagRequirementIdsToBeVoided,
                List<int> requirementDefinitionIdsToBeAdded,
                CancellationToken token)
                => await tagValidator.RequirementUsageWillCoverBothForSupplierAndOtherAsync(
                    tagId, 
                    tagRequirementIdsToBeUnvoided, 
                    tagRequirementIdsToBeVoided, 
                    requirementDefinitionIdsToBeAdded, 
                    token);
            
            async Task<bool> RequirementUsageIsForJourneysWithoutSupplierAsync(
                int tagId, 
                List<int> tagRequirementIdsToBeUnvoided,
                List<int> tagRequirementIdsToBeVoided,
                List<int> requirementDefinitionIdsToBeAdded,
                CancellationToken token)
                => await tagValidator.RequirementUsageWillCoverForOtherThanSuppliersAsync(
                    tagId, 
                    tagRequirementIdsToBeUnvoided, 
                    tagRequirementIdsToBeVoided, 
                    requirementDefinitionIdsToBeAdded, 
                    token);
            
            async Task<bool> IsASupplierStepAsync(int stepId, CancellationToken token)
                => await stepValidator.IsForSupplierAsync(stepId, token);
            
            async Task<bool> NotBeAClosedProjectForTagAsync(int tagId, CancellationToken token)
                => !await projectValidator.IsClosedForTagAsync(tagId, token);
            
            async Task<bool> IsAnAreaTagAsync(int tagId, CancellationToken token)
                => await tagValidator.VerifyTagIsAreaTagAsync(tagId, token);
            
            async Task<bool> BeAnExistingTagAsync(int tagId, CancellationToken token)
                => await tagValidator.ExistsAsync(tagId, token);
            
            async Task<bool> NotBeAVoidedTagAsync(int tagId, CancellationToken token)
                => !await tagValidator.IsVoidedAsync(tagId, token);
            
            async Task<bool> NotBeAPoAreaTagAsync(int tagId, CancellationToken token)
                => !await tagValidator.VerifyTagTypeAsync(tagId, TagType.PoArea, token);

            async Task<bool> NotChangedToAVoidedStepAsync(int tagId, int stepId, CancellationToken token)
                => await tagValidator.HasStepAsync(tagId, stepId, token) ||
                   !await stepValidator.IsVoidedAsync(stepId, token);
            
            async Task<bool> BeAnExistingRequirementDefinitionAsync(int requirementDefinitionId, CancellationToken token)
                => await requirementDefinitionValidator.ExistsAsync(requirementDefinitionId, token);
            
            async Task<bool> NotBeAVoidedRequirementDefinitionAsync(int requirementDefinitionId, CancellationToken token)
                => !await requirementDefinitionValidator.IsVoidedAsync(requirementDefinitionId, token);
            
            async Task<bool> BeAnExistingTagRequirementAsync(int tagId, int tagRequirementId, CancellationToken token)
                => await tagValidator.HasRequirementAsync(tagId, tagRequirementId, token);
            
            async Task<bool> BeAVoidedTagRequirementAsync(
                int tagId,
                int tagRequirementId,
                List<int> tagRequirementIdsToBeVoided,
                CancellationToken token)
            {
                if (tagRequirementIdsToBeVoided.Contains(tagRequirementId))
                {
                    return true;
                }

                return await tagValidator.IsRequirementVoidedAsync(tagId, tagRequirementId, token);
            }

            bool HaveAValidRowVersion(string rowVersion)
                => rowVersionValidator.IsValid(rowVersion);

            async Task<bool> NotChangeDescriptionAsync(int tagId, string description, CancellationToken token)
                => description == null || await tagValidator.VerifyTagDescriptionAsync(tagId, description, token);
        }
    }
}
