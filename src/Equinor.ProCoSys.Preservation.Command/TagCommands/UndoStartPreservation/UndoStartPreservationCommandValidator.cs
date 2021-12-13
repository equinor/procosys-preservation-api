using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Validators.TagValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.ProjectValidators;
using FluentValidation;

namespace Equinor.ProCoSys.Preservation.Command.TagCommands.UndoStartPreservation
{
    public class UndoStartPreservationCommandValidator : AbstractValidator<UndoStartPreservationCommand>
    {
        public UndoStartPreservationCommandValidator(
            IProjectValidator projectValidator,
            ITagValidator tagValidator)
        {
            CascadeMode = CascadeMode.Stop;
                        
            RuleFor(command => command.TagIds)
                .Must(ids => ids != null && ids.Any())
                .WithMessage("At least 1 tag must be given!")
                .Must(BeUniqueTags)
                .WithMessage("Tags must be unique!");

            When(command => command.TagIds.Any() && BeUniqueTags(command.TagIds), () =>
            {
                RuleForEach(command => command.TagIds)
                    .MustAsync((_, tagId, _, token) => BeAnExistingTagAsync(tagId, token))
                    .WithMessage((_, tagId) => $"Tag doesn't exist! Tag={tagId}")
                    .MustAsync((_, tagId, _, token) => NotBeAVoidedTagAsync(tagId, token))
                    .WithMessage((_, tagId) => $"Tag is voided! Tag={tagId}")
                    .MustAsync((_, tagId, _, token) => IsStartedAsync(tagId, token))
                    .WithMessage((_, tagId) => $"Preservation on tag is not started! Tag={tagId}");
            });

            RuleFor(command => command.TagIds)
                .MustAsync(BeInSameProjectAsync)
                .WithMessage("Tags must be in same project!")
                .MustAsync(NotBeAClosedProjectForTagAsync)
                .WithMessage("Project is closed!");

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

            async Task<bool> IsStartedAsync(int tagId, CancellationToken token)
                => await tagValidator.IsStartedAsync(tagId, token);
        }
    }
}
