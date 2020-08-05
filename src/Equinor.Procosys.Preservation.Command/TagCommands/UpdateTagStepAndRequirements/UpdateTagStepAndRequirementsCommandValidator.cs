using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators;
using Equinor.Procosys.Preservation.Command.Validators.ProjectValidators;
using Equinor.Procosys.Preservation.Command.Validators.RequirementDefinitionValidators;
using Equinor.Procosys.Preservation.Command.Validators.StepValidators;
using Equinor.Procosys.Preservation.Command.Validators.TagValidators;
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
            CascadeMode = CascadeMode.StopOnFirstFailure;

            WhenAsync((command, token) => BeASupplierStepAsync(command.StepId, token), () =>
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
                            command.UpdatedRequirements.Where(u => u.IsVoided).Select(u => u.TagRequirementId).ToList(),
                            command.NewRequirements.Select(r => r.RequirementDefinitionId).ToList(),
                            token))
                    .WithMessage(command => "Requirements must include requirements to be used both for supplier and other than suppliers!");
            }).Otherwise(() =>
            {
                RuleFor(command => command)
                    .MustAsync((_, command, token) =>
                        RequirementsMustBeUniqueAfterUpdateAsync(
                            command.TagId,
                            command.NewRequirements.Select(r => r.RequirementDefinitionId).ToList(),
                            token))
                    .WithMessage(command => "Requirements must be unique!")
                    .MustAsync((_, command, token) =>
                        RequirementUsageIsForJourneysWithoutSupplierAsync(
                            command.TagId,
                            command.UpdatedRequirements.Where(u => u.IsVoided).Select(u => u.TagRequirementId).ToList(),
                            command.NewRequirements.Select(r => r.RequirementDefinitionId).ToList(),
                            token))
                    .WithMessage(command => "Requirements must include requirements to be used for other than suppliers!")
                    .MustAsync((_, command, token) =>
                        RequirementUsageIsNotForSupplierStepOnlyAsync(
                            command.TagId,
                            command.UpdatedRequirements.Where(u => u.IsVoided).Select(u => u.TagRequirementId).ToList(),
                            command.NewRequirements.Select(r => r.RequirementDefinitionId).ToList(),
                            token))
                    .WithMessage(command => "Requirements can not include requirements just for suppliers!");
            });

            RuleFor(command => command)
                .MustAsync((command, token) => NotBeAClosedProjectForTagAsync(command.TagId, token))
                .WithMessage(command => $"Project for tag is closed! Tag={command.TagId}")
                .MustAsync((command, token) => BeAnExistingTagAsync(command.TagId, token))
                .WithMessage(command => $"Tag doesn't exist! Tag={command.TagId}")
                .MustAsync((command, token) => NotBeAVoidedTagAsync(command.TagId, token))
                .WithMessage(command => $"Tag is voided! Tag={command.TagId}")
                .MustAsync((command, token) => HaveAValidRowVersion(command.RowVersion, token))
                .WithMessage(command => $"Not a valid RowVersion! RowVersion={command.RowVersion}");

            RuleForEach(command => command.UpdatedRequirements)
                .MustAsync((command, req, __, token) => BeAnExistingTagRequirementAsync(command.TagId, req.TagRequirementId, token))
                .WithMessage((_, req) => $"Requirement doesn't exists! Requirement={req.TagRequirementId}");
            
            RuleForEach(command => command.NewRequirements)
                .MustAsync((_, req, __, token) => BeAnExistingRequirementDefinitionAsync(req.RequirementDefinitionId, token))
                .WithMessage((_, req) =>
                    $"Requirement definition doesn't exists! Requirement={req.RequirementDefinitionId}")
                .MustAsync((_, req, __, token) => NotBeAVoidedRequirementDefinitionAsync(req.RequirementDefinitionId, token))
                .WithMessage((_, req) =>
                    $"Requirement definition is voided! Requirement={req.RequirementDefinitionId}");

            async Task<bool> RequirementsMustBeUniqueAfterUpdateAsync(int tagId, List<int> requirementDefinitionIdsToBeAdded, CancellationToken token)
                => requirementDefinitionIdsToBeAdded.Count == 0 || 
                   await tagValidator.AllRequirementsWillBeUniqueAsync(tagId, requirementDefinitionIdsToBeAdded, token);
            async Task<bool> RequirementUsageIsForAllJourneysAsync(
                int tagId, 
                List<int> tagRequirementIdsToBeVoided,
                List<int> requirementDefinitionIdsToBeAdded,
                CancellationToken token)
                => await tagValidator.UsageCoversBothForSupplierAndOtherAsync(tagId, tagRequirementIdsToBeVoided, requirementDefinitionIdsToBeAdded, token);
            async Task<bool> RequirementUsageIsForJourneysWithoutSupplierAsync(
                int tagId, 
                List<int> tagRequirementIdsToBeVoided,
                List<int> requirementDefinitionIdsToBeAdded,
                CancellationToken token)
                => await tagValidator.UsageCoversForOtherThanSuppliersAsync(tagId, tagRequirementIdsToBeVoided, requirementDefinitionIdsToBeAdded, token);
            async Task<bool> RequirementUsageIsNotForSupplierStepOnlyAsync(
                int tagId, 
                List<int> tagRequirementIdsToBeVoided,
                List<int> requirementDefinitionIdsToBeAdded,
                CancellationToken token)
                => !await tagValidator.HasAnyForSupplierOnlyUsageAsync(tagId, tagRequirementIdsToBeVoided, requirementDefinitionIdsToBeAdded, token);
            async Task<bool> BeASupplierStepAsync(int stepId, CancellationToken token)
                => await stepValidator.IsForSupplierAsync(stepId, token);
            async Task<bool> NotBeAClosedProjectForTagAsync(int tagId, CancellationToken token)
                => !await projectValidator.IsClosedForTagAsync(tagId, token);
            async Task<bool> BeAnExistingTagAsync(int tagId, CancellationToken token)
                => await tagValidator.ExistsAsync(tagId, token);
            async Task<bool> NotBeAVoidedTagAsync(int tagId, CancellationToken token)
                => !await tagValidator.IsVoidedAsync(tagId, token);
            async Task<bool> BeAnExistingRequirementDefinitionAsync(int requirementDefinitionId, CancellationToken token)
                => await requirementDefinitionValidator.ExistsAsync(requirementDefinitionId, token);
            async Task<bool> NotBeAVoidedRequirementDefinitionAsync(int requirementDefinitionId, CancellationToken token)
                => !await requirementDefinitionValidator.IsVoidedAsync(requirementDefinitionId, token);
            async Task<bool> BeAnExistingTagRequirementAsync(int tagId, int tagRequirementId, CancellationToken token)
                => await tagValidator.HasRequirementAsync(tagId, tagRequirementId, token);
            async Task<bool> HaveAValidRowVersion(string rowVersion, CancellationToken token)
                => await rowVersionValidator.IsValid(rowVersion, token);
        }
    }
}
