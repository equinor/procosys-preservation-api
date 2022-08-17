using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Validators;
using Equinor.ProCoSys.Preservation.Command.Validators.ProjectValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.StepValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.TagValidators;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using FluentValidation;

namespace Equinor.ProCoSys.Preservation.Command.TagCommands.UpdateTagStep
{
    public class UpdateTagStepCommandValidator : AbstractValidator<UpdateTagStepCommand>
    {
        public UpdateTagStepCommandValidator(
             IProjectValidator projectValidator,
             ITagValidator tagValidator,
             IStepValidator stepValidator,
             IRowVersionValidator rowVersionValidator)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(command => command.Tags)
                .Must(ids => ids != null && ids.Any())
                .WithMessage("At least 1 tag must be given!")
                .Must(BeUniqueTags)
                .WithMessage("Tags must be unique!");

            RuleFor(command => command.StepId)
                .MustAsync(BeAnExistingStepAsync)
                .WithMessage(command => $"Step doesn't exist! Step={command.StepId}")
                .MustAsync(NotBeAVoidedStepAsync)
                .WithMessage(command => $"Step is voided! Step={command.StepId}");

            When(command => command.Tags.Any() && BeUniqueTags(command.Tags), () =>
            {
                RuleForEach(command => command.Tags)
                    .MustAsync((_, tag, _, token) => BeAnExistingTagAsync(tag.Id, token))
                    .WithMessage((_, tag) => $"Tag doesn't exist! TagId={tag.Id}")
                    .MustAsync((_, tag, _, token) => BeReadyForEditingAsync(tag.Id, token))
                    .WithMessage((_, tag) => $"Tag can't be edited! Tag='{GetTagDetails(tag.Id)}'")
                    .MustAsync((_, tag, _, token) => NotBeAVoidedTagAsync(tag.Id, token))
                    .WithMessage((_, tag) => $"Tag is voided! Tag='{GetTagDetails(tag.Id)}'")
                    .Must(tag => HaveAValidRowVersion(tag.RowVersion))
                    .WithMessage((_, tag) => $"Not a valid row version! Row version={tag.RowVersion}");

                WhenAsync((command, token) => IsASupplierStepAsync(command.StepId, token), () =>
                {
                    WhenAsync((command, token) => NotBeAPoAreaTagAsync(command.TagId, token), () =>
                    {
                        RuleForEach(command => command.Tags)
                            .MustAsync((_, tag, _, token) => RequirementUsageCoversBothForSupplierAndOtherAsync(tag.Id, token))
                            .WithMessage((_, tag) => $"Requirements for tag must include requirements to be used both for supplier and other than suppliers! Tag='{GetTagDetails(tag.Id)}'");
                    }).Otherwise(() =>
                    {
                        RuleForEach(command => command.Tags)
                            .MustAsync((_, tag, _, token) => RequirementUsageCoversForSuppliersAsync(tag.Id, token))
                            .WithMessage((_, tag) => $"Requirements for tag must include requirements to be used for supplier! Tag='{GetTagDetails(tag.Id)}'")
                            .MustAsync((_, tag, _, token) => NotHaveRequirementsForOtherThanSupplierAsync(tag.Id, token))
                            .WithMessage((_, tag) => $"Requirements for tag can not include requirements for other than suppliers! Tag='{GetTagDetails(tag.Id)}'");
                    });
                }).Otherwise(() =>
                {
                    RuleForEach(command => command.Tags)
                        .MustAsync((_, tag, _, token) => NotBeAPoAreaTagAsync(tag.Id, token))
                        .WithMessage((_, tag) => $"Step for a {TagType.PoArea.GetTagNoPrefix()} tag needs to be for supplier! Tag='{GetTagDetails(tag.Id)}'")
                        .MustAsync((_, tag, _, token) => RequirementUsageCoversForOtherThanSuppliersAsync(tag.Id, token))
                        .WithMessage((_, tag) => $"Requirements for tag must include requirements to be used for other than suppliers! Tag='{GetTagDetails(tag.Id)}'");
                });
            });

            RuleFor(command => command.Tags)
                .MustAsync(BeInSameProjectAsync)
                .WithMessage("Tags must be in same project!")
                .MustAsync(NotBeAClosedProjectForTagAsync)
                .WithMessage("Project is closed!");

            bool BeUniqueTags(IEnumerable<IdAndRowVersion> tags)
            {
                var ids = tags.Select(x => x.Id).ToList();
                return ids.Distinct().Count() == ids.Count;
            }

            string GetTagDetails(int tagId) => tagValidator.GetTagDetailsAsync(tagId, default).Result;

            async Task<bool> BeAnExistingStepAsync(int stepId, CancellationToken token)
                => await stepValidator.ExistsAsync(stepId, token);

            async Task<bool> NotBeAVoidedStepAsync(int stepId, CancellationToken token)
                => !await stepValidator.IsVoidedAsync(stepId, token);

            async Task<bool> BeInSameProjectAsync(IEnumerable<IdAndRowVersion> tags, CancellationToken token)
                => await projectValidator.AllTagsInSameProjectAsync(tags.Select(t => t.Id), token);

            async Task<bool> NotBeAClosedProjectForTagAsync(IEnumerable<IdAndRowVersion> tags, CancellationToken token)
                => !await projectValidator.IsClosedForTagAsync(tags.First().Id, token);

            async Task<bool> NotHaveRequirementsForOtherThanSupplierAsync(int tagId, CancellationToken token)
                => !await tagValidator.RequirementHasAnyForOtherThanSuppliersUsageAsync(tagId, token);
            
            async Task<bool> RequirementUsageCoversForSuppliersAsync(int tagId, CancellationToken token)
                => await tagValidator.RequirementUsageCoversForSuppliersAsync(tagId, token);
            
            async Task<bool> RequirementUsageCoversBothForSupplierAndOtherAsync(int tagId, CancellationToken token)
                => await tagValidator.RequirementUsageCoversBothForSupplierAndOtherAsync(tagId, token);
            
            async Task<bool> RequirementUsageCoversForOtherThanSuppliersAsync(int tagId, CancellationToken token)
                => await tagValidator.RequirementUsageCoversForOtherThanSuppliersAsync(tagId, token);
            
            async Task<bool> IsASupplierStepAsync(int stepId, CancellationToken token)
                => await stepValidator.IsForSupplierAsync(stepId, token);

            async Task<bool> BeAnExistingTagAsync(int tagId, CancellationToken token)
                => await tagValidator.ExistsAsync(tagId, token);

            async Task<bool> BeReadyForEditingAsync(int tagId, CancellationToken token)
                => await tagValidator.IsReadyToBeEditedAsync(tagId, token);

            async Task<bool> NotBeAVoidedTagAsync(int tagId, CancellationToken token)
                => !await tagValidator.IsVoidedAsync(tagId, token);
            
            async Task<bool> NotBeAPoAreaTagAsync(int tagId, CancellationToken token)
                => !await tagValidator.VerifyTagTypeAsync(tagId, TagType.PoArea, token);

            bool HaveAValidRowVersion(string rowVersion)
                => rowVersionValidator.IsValid(rowVersion);
        }
    }
}
