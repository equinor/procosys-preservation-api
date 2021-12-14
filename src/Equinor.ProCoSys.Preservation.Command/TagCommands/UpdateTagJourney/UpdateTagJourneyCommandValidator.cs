﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Validators;
using Equinor.ProCoSys.Preservation.Command.Validators.ProjectValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.StepValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.TagValidators;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using FluentValidation;

namespace Equinor.ProCoSys.Preservation.Command.TagCommands.UpdateTagJourney
{
    public class UpdateTagJourneyCommandValidator : AbstractValidator<UpdateTagJourneyCommand>
    {
        public UpdateTagJourneyCommandValidator(
             IProjectValidator projectValidator,
             ITagValidator tagValidator,
             IStepValidator stepValidator,
             IRowVersionValidator rowVersionValidator)
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(command => command.Tags)
                .Must(ids => ids != null && ids.Any())
                .WithMessage("At least 1 tag must be given!")
                .Must(BeUniqueTags)
                .WithMessage("Tags must be unique!");

            When(command => command.Tags.Any() && BeUniqueTags(command.Tags), () =>
            {
                RuleForEach(command => command.Tags)
                    .MustAsync((_, tag, _, token) => BeAnExistingTagAsync(tag.Id, token))
                    .WithMessage((_, tag) => $"Tag doesn't exist! Tag={tag.Id}")
                    .MustAsync((_, tag, _, token) => NotBeAVoidedTagAsync(tag.Id, token))
                    .WithMessage((_, tag) => $"Tag is voided! Tag={tag.Id}")
                    .Must(tag => HaveAValidRowVersion(tag.RowVersion))
                    .WithMessage((_, tag) => $"Not a valid row version! Row version={tag.RowVersion}");


                WhenAsync((command, token) => IsASupplierStepAsync(command.StepId, token), () =>
                {
                    WhenAsync((command, token) => NotBeAPoAreaTagAsync(command.TagId, token), () =>
                    {
                        RuleFor(command => command)
                            .MustAsync((_, command, token) =>
                                HaveRequirementsForSupplierAndOtherAsync(command.TagId, token))
                            .WithMessage(_ => "Requirements must include requirements to be used both for supplier and other than suppliers!");
                    }).Otherwise(() =>
                    {
                        RuleFor(command => command)
                            .MustAsync((_, command, token) =>
                                HaveRequirementsForSupplierAsync(command.TagId, token))
                            .WithMessage(_ => "Requirements must include requirements to be used for supplier!")
                            .MustAsync((command, token) => NotHaveRequirementsForOtherThanSupplierAsync(command.TagId, token))
                            .WithMessage(_ => "Requirements can not include requirements for other than suppliers!");
                    });
                }).Otherwise(() =>
                {
                    RuleFor(command => command)
                        .MustAsync((command, token) => NotBeAPoAreaTagAsync(command.TagId, token))
                        .WithMessage(_ => $"Step for a {TagType.PoArea.GetTagNoPrefix()} tag needs to be for supplier!")
                        .MustAsync((_, command, token) =>
                            HaveRequirementsForOtherThanSupplierAsync(command.TagId, token))
                        .WithMessage(_ => "Requirements must include requirements to be used for other than suppliers!");
                });
            });

            RuleFor(command => command.Tags)
                .MustAsync((command, _, _, token) => NotUpdateToAVoidedStepAsync(command.TagId, command.StepId, token))
                .WithMessage(command => $"Step is voided! Step={command.StepId}")
                .MustAsync(BeInSameProjectAsync)
                .WithMessage("Tags must be in same project!")
                .MustAsync(NotBeAClosedProjectForTagAsync)
                .WithMessage("Project is closed!");

            bool BeUniqueTags(IEnumerable<IdAndRowVersion> tags)
            {
                var ids = tags.Select(x => x.Id).ToList();
                return ids.Distinct().Count() == ids.Count;
            }

            async Task<bool> BeInSameProjectAsync(IEnumerable<IdAndRowVersion> tags, CancellationToken token)
                => await projectValidator.AllTagsInSameProjectAsync(tags.Select(t => t.Id), token);

            async Task<bool> NotBeAClosedProjectForTagAsync(IEnumerable<IdAndRowVersion> tags, CancellationToken token)
                => !await projectValidator.IsClosedForTagAsync(tags.First().Id, token);

            async Task<bool> NotHaveRequirementsForOtherThanSupplierAsync(int tagId, CancellationToken token)
                => !await tagValidator.HasRequirementsForOtherThanSuppliersAsync(tagId, token);
            
            async Task<bool> HaveRequirementsForSupplierAsync(int tagId, CancellationToken token)
                => await tagValidator.HasRequirementsForSuppliersAsync(tagId, token);
            
            async Task<bool> HaveRequirementsForSupplierAndOtherAsync(int tagId, CancellationToken token)
                => await tagValidator.HasRequirementsForBothSupplierAndOtherAsync(tagId, token);
            
            async Task<bool> HaveRequirementsForOtherThanSupplierAsync(int tagId, CancellationToken token)
                => await tagValidator.HasRequirementsForOtherThanSuppliersAsync(tagId, token);
            
            async Task<bool> IsASupplierStepAsync(int stepId, CancellationToken token)
                => await stepValidator.IsForSupplierAsync(stepId, token);

            async Task<bool> BeAnExistingTagAsync(int tagId, CancellationToken token)
                => await tagValidator.ExistsAsync(tagId, token);
            
            async Task<bool> NotBeAVoidedTagAsync(int tagId, CancellationToken token)
                => !await tagValidator.IsVoidedAsync(tagId, token);
            
            async Task<bool> NotBeAPoAreaTagAsync(int tagId, CancellationToken token)
                => !await tagValidator.VerifyTagTypeAsync(tagId, TagType.PoArea, token);

            async Task<bool> NotUpdateToAVoidedStepAsync(int tagId, int stepId, CancellationToken token)
                => await tagValidator.HasStepAsync(tagId, stepId, token) ||
                   !await stepValidator.IsVoidedAsync(stepId, token);

            bool HaveAValidRowVersion(string rowVersion)
                => rowVersionValidator.IsValid(rowVersion);
        }
    }
}
