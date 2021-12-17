using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Validators;
using Equinor.ProCoSys.Preservation.Command.Validators.ProjectValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.RequirementDefinitionValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.TagValidators;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using FluentValidation;

namespace Equinor.ProCoSys.Preservation.Command.TagCommands.UpdateTagRequirements
{
    public class UpdateTagRequirementsCommandValidator : AbstractValidator<UpdateTagRequirementsCommand>
    {
        public UpdateTagRequirementsCommandValidator(
             IProjectValidator projectValidator,
             ITagValidator tagValidator,
             IRequirementDefinitionValidator requirementDefinitionValidator,
             IRowVersionValidator rowVersionValidator)
        {
            CascadeMode = CascadeMode.Stop;

            WhenAsync((command, token) => TagIsInASupplierStepAsync(command.TagId, token), () =>
            {
                WhenAsync((command, token) => NotBeAPoAreaTagAsync(command.TagId, token), () =>
                {
                    RuleFor(command => command)
                        .MustAsync((_, command, token) =>
                            RequirementUsageWillCoverBothForSupplierAndOtherAsync(
                                command.TagId,
                                command.UpdatedRequirements.Where(u => !u.IsVoided).Select(u => u.TagRequirementId).ToList(),
                                command.UpdatedRequirements.Where(u => u.IsVoided).Select(u => u.TagRequirementId).ToList(),
                                command.NewRequirements.Select(r => r.RequirementDefinitionId).ToList(),
                                token))
                        .WithMessage(_ => "Requirements must include requirements to be used both for supplier and other than suppliers!");
                }).Otherwise(() =>
                {
                    RuleFor(command => command)
                        .MustAsync((_, command, token) =>
                            RequirementUsageWillCoverForSuppliersAsync(
                                command.TagId,
                                command.UpdatedRequirements.Where(u => !u.IsVoided).Select(u => u.TagRequirementId).ToList(),
                                command.UpdatedRequirements.Where(u => u.IsVoided).Select(u => u.TagRequirementId).ToList(),
                                command.NewRequirements.Select(r => r.RequirementDefinitionId).ToList(),
                                token))
                        .WithMessage(_ => "Requirements must include requirements to be used for supplier!")
                        .MustAsync((command, token) => WillNotGetAnyRequirementForOtherThanSuppliersUsageAsync(
                            command.TagId,
                            command.UpdatedRequirements.Where(u => !u.IsVoided).Select(u => u.TagRequirementId).ToList(),
                            command.UpdatedRequirements.Where(u => u.IsVoided).Select(u => u.TagRequirementId).ToList(),
                            command.NewRequirements.Select(r => r.RequirementDefinitionId).ToList(),
                            token))
                        .WithMessage(_ => "Requirements can not include requirements for other than suppliers!");
                });
            }).Otherwise(() =>
            {
                RuleFor(command => command)
                    .MustAsync((command, token) => NotBeAPoAreaTagAsync(command.TagId, token))
                    .WithMessage(_ => $"Step for a {TagType.PoArea.GetTagNoPrefix()} tag needs to be for supplier!")
                    .MustAsync((_, command, token) =>
                        RequirementUsageWillCoverForOtherThanSuppliersAsync(
                            command.TagId,
                            command.UpdatedRequirements.Where(u => !u.IsVoided).Select(u => u.TagRequirementId).ToList(),
                            command.UpdatedRequirements.Where(u => u.IsVoided).Select(u => u.TagRequirementId).ToList(),
                            command.NewRequirements.Select(r => r.RequirementDefinitionId).ToList(),
                            token))
                    .WithMessage(_ => "Requirements must include requirements to be used for other than suppliers!");
            });

            WhenAsync((command, token) => IsAnAreaTagAsync(command.TagId, token), () =>
            {
                RuleFor(command => command)
                    .Must(command => !string.IsNullOrEmpty(command.Description))
                    .WithMessage(_ => "Description can not be blank!");
            }).Otherwise(() =>
            {
                RuleFor(command => command)
                    .MustAsync((command, token)
                        => NotChangeDescriptionAsync(command.TagId, command.Description, token))
                    .WithMessage(_ => "Tag must be an area tag to update description!");
            });

            RuleFor(command => command)
                .MustAsync((_, command, token) =>
                    RequirementsMustBeUniqueAfterUpdateAsync(
                        command.TagId,
                        command.NewRequirements.Select(r => r.RequirementDefinitionId).ToList(),
                        token))
                .WithMessage(_ => "Requirements must be unique!")
                .MustAsync((command, token) => NotBeAClosedProjectForTagAsync(command.TagId, token))
                .WithMessage(command => $"Project for tag is closed! Tag={command.TagId}")
                .MustAsync((command, token) => BeAnExistingTagAsync(command.TagId, token))
                .WithMessage(command => $"Tag doesn't exist! Tag={command.TagId}")
                .MustAsync((command, token) => BeReadyForEditingAsync(command.TagId, token))
                .WithMessage(command => $"Tag can't be edited! Tag={command.TagId}")
                .MustAsync((command, token) => NotBeAVoidedTagAsync(command.TagId, token))
                .WithMessage(command => $"Tag is voided! Tag={command.TagId}")
                .Must(command => HaveAValidRowVersion(command.RowVersion))
                .WithMessage(command => $"Not a valid row version! Row version={command.RowVersion}");

            RuleForEach(command => command.UpdatedRequirements)
                .MustAsync((command, req, _, token) => BeAnExistingTagRequirementAsync(command.TagId, req.TagRequirementId, token))
                .WithMessage((_, req) => $"Requirement doesn't exist! Requirement={req.TagRequirementId}");

            RuleForEach(command => command.DeletedRequirements)
                .MustAsync((command, req, _, token) => BeAnExistingTagRequirementAsync(command.TagId, req.TagRequirementId, token))
                .WithMessage((_, req) => $"Requirement doesn't exist! Requirement={req.TagRequirementId}")
                .MustAsync((command, req, _, token) => BeAVoidedTagRequirementAsync(
                    command.TagId,
                    req.TagRequirementId,
                    command.UpdatedRequirements.Where(u => u.IsVoided).Select(u => u.TagRequirementId).ToList(),
                    token))
                .WithMessage((_, req) => $"Requirement is not voided! Requirement={req.TagRequirementId}");
            
            RuleForEach(command => command.NewRequirements)
                .MustAsync((_, req, _, token) => BeAnExistingRequirementDefinitionAsync(req.RequirementDefinitionId, token))
                .WithMessage((_, req) =>
                    $"Requirement definition doesn't exist! Requirement definition={req.RequirementDefinitionId}")
                .MustAsync((_, req, _, token) => NotBeAVoidedRequirementDefinitionAsync(req.RequirementDefinitionId, token))
                .WithMessage((_, req) =>
                    $"Requirement definition is voided! Requirement definition={req.RequirementDefinitionId}");

            async Task<bool> RequirementsMustBeUniqueAfterUpdateAsync(
                int tagId,
                List<int> requirementDefinitionIdsToBeAdded,
                CancellationToken token)
                => requirementDefinitionIdsToBeAdded.Count == 0 ||
                   await tagValidator.AllRequirementsWillBeUniqueAsync(tagId, requirementDefinitionIdsToBeAdded, token);
            
            async Task<bool> WillNotGetAnyRequirementForOtherThanSuppliersUsageAsync(
                int tagId, 
                List<int> tagRequirementIdsToBeUnvoided,
                List<int> tagRequirementIdsToBeVoided,
                List<int> requirementDefinitionIdsToBeAdded,
                CancellationToken token)
                => !await tagValidator.RequirementWillGetAnyForOtherThanSuppliersUsageAsync(
                    tagId, 
                    tagRequirementIdsToBeUnvoided, 
                    tagRequirementIdsToBeVoided, 
                    requirementDefinitionIdsToBeAdded, 
                    token);
            
            async Task<bool> RequirementUsageWillCoverForSuppliersAsync(
                int tagId, 
                List<int> tagRequirementIdsToBeUnvoided,
                List<int> tagRequirementIdsToBeVoided,
                List<int> requirementDefinitionIdsToBeAdded,
                CancellationToken token)
                => await tagValidator.RequirementUsageWillCoverForSuppliersAsync(
                    tagId, 
                    tagRequirementIdsToBeUnvoided, 
                    tagRequirementIdsToBeVoided, 
                    requirementDefinitionIdsToBeAdded, 
                    token);
            
            async Task<bool> RequirementUsageWillCoverBothForSupplierAndOtherAsync(
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
            
            async Task<bool> RequirementUsageWillCoverForOtherThanSuppliersAsync(
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
            
            async Task<bool> TagIsInASupplierStepAsync(int tagId, CancellationToken token)
                => await tagValidator.IsInASupplierStepAsync(tagId, token);
            
            async Task<bool> NotBeAClosedProjectForTagAsync(int tagId, CancellationToken token)
                => !await projectValidator.IsClosedForTagAsync(tagId, token);
            
            async Task<bool> IsAnAreaTagAsync(int tagId, CancellationToken token)
                => await tagValidator.VerifyTagIsAreaTagAsync(tagId, token);
            
            async Task<bool> BeAnExistingTagAsync(int tagId, CancellationToken token)
                => await tagValidator.ExistsAsync(tagId, token);
            
            async Task<bool> BeReadyForEditingAsync(int tagId, CancellationToken token)
                => await tagValidator.IsReadyToBeEditedAsync(tagId, token);

            async Task<bool> NotBeAVoidedTagAsync(int tagId, CancellationToken token)
                => !await tagValidator.IsVoidedAsync(tagId, token);
            
            async Task<bool> NotBeAPoAreaTagAsync(int tagId, CancellationToken token)
                => !await tagValidator.VerifyTagTypeAsync(tagId, TagType.PoArea, token);
            
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
