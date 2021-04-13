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
            CascadeMode = CascadeMode.Stop;
                        
            RuleFor(command => command.TagIds)
                .Must(ids => ids != null && ids.Any())
                .WithMessage("At least 1 tag must be given!")
                .Must(BeUniqueTags)
                .WithMessage("Tags must be unique!")
                .MustAsync(BeInSameProjectAsync)
                .WithMessage("Tags must be in same project!")
                .MustAsync(NotBeAClosedProjectForTagAsync)
                .WithMessage("Project is closed!");

            When(command => command.TagIds.Any() && BeUniqueTags(command.TagIds), () =>
            {
                RuleForEach(command => command.TagIds)
                    .MustAsync((_, tagId, __, token) => BeAnExistingTagAsync(tagId, token))
                    .WithMessage((_, id) => $"Tag doesn't exist! Tag={id}")
                    .MustAsync((_, tagId, __, token) => NotBeAVoidedTagAsync(tagId, token))
                    .WithMessage((_, id) => $"Tag is voided! Tag={id}")
                    .MustAsync((_, tagId, __, token) => PreservationIsStartedAsync(tagId, token))
                    .WithMessage((_, id) => $"Tag must have status {PreservationStatus.Active} to preserve! Tag={id}")
                    .MustAsync((_, tagId, __, token) => BeReadyToBePreservedAsync(tagId, token))
                    .WithMessage((_, id) => $"Tag is not ready to be bulk preserved! Tag={id}");
            });

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
