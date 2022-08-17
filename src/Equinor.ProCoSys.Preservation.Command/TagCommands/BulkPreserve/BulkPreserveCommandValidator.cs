using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Validators.ProjectValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.TagValidators;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using FluentValidation;

namespace Equinor.ProCoSys.Preservation.Command.TagCommands.BulkPreserve
{
    public class BulkPreserveCommandValidator : AbstractValidator<BulkPreserveCommand>
    {
        public BulkPreserveCommandValidator(
            IProjectValidator projectValidator,
            ITagValidator tagValidator)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(command => command.TagIds)
                .Must(ids => ids != null && ids.Any())
                .WithMessage("At least 1 tag must be given!")
                .Must(BeUniqueTags)
                .WithMessage("Tags must be unique!");

            When(command => command.TagIds.Any() && BeUniqueTags(command.TagIds), () =>
            {
                RuleForEach(command => command.TagIds)
                    .MustAsync((_, tagId, _, token) => BeAnExistingTagAsync(tagId, token))
                    .WithMessage((_, tagId) => $"Tag doesn't exist! TagId={tagId}")
                    .MustAsync((_, tagId, _, token) => NotBeAVoidedTagAsync(tagId, token))
                    .WithMessage((_, tagId) => $"Tag is voided! Tag='{GetTagDetails(tagId)}'")
                    .MustAsync((_, tagId, _, token) => PreservationIsStartedAsync(tagId, token))
                    .WithMessage((_, tagId) => $"Tag must have status {PreservationStatus.Active} to preserve! Tag='{GetTagDetails(tagId)}'")
                    .MustAsync((_, tagId, _, token) => BeReadyToBePreservedAsync(tagId, token))
                    .WithMessage((_, tagId) => $"Tag is not ready to be bulk preserved! Tag='{GetTagDetails(tagId)}'");
            });

            RuleFor(command => command.TagIds)
                .MustAsync(BeInSameProjectAsync)
                .WithMessage("Tags must be in same project!")
                .MustAsync(NotBeAClosedProjectForTagAsync)
                .WithMessage("Project is closed!");

            string GetTagDetails(int tagId) => tagValidator.GetTagDetailsAsync(tagId, default).Result;

            bool BeUniqueTags(IEnumerable<int> tagIds)
            {
                var ids = tagIds.ToList();
                return ids.Distinct().Count() == ids.Count;
            }
            
            async Task<bool> BeInSameProjectAsync(IEnumerable<int> tagIds, CancellationToken token)
                => await projectValidator.AllTagsInSameProjectAsync(tagIds, token);
            
            async Task<bool> NotBeAClosedProjectForTagAsync(IEnumerable<int> tagIds, CancellationToken token)
                => !await projectValidator.IsClosedForTagAsync(tagIds.First(), token);

            async Task<bool> BeAnExistingTagAsync(int tagId, CancellationToken token)
                => await tagValidator.ExistsAsync(tagId, token);

            async Task<bool> NotBeAVoidedTagAsync(int tagId, CancellationToken token)
                => !await tagValidator.IsVoidedAsync(tagId, token);

            async Task<bool> PreservationIsStartedAsync(int tagId, CancellationToken token)
                => await tagValidator.VerifyPreservationStatusAsync(tagId, PreservationStatus.Active, token);
            
            async Task<bool> BeReadyToBePreservedAsync(int tagId, CancellationToken token)
                => await tagValidator.IsReadyToBePreservedAsync(tagId, token);
        }
    }
}
