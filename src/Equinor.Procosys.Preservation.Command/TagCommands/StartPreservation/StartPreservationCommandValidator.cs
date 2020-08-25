using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.TagValidators;
using Equinor.Procosys.Preservation.Command.Validators.ProjectValidators;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.TagCommands.StartPreservation
{
    public class StartPreservationCommandValidator : AbstractValidator<StartPreservationCommand>
    {
        public StartPreservationCommandValidator(
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
                    .MustAsync((_, tagId, __, token) => IsReadyToBeStartedAsync(tagId, token))
                    .WithMessage((_, id) => $"Preservation on tag can not be started! Tag={id}")
                    .MustAsync((_, tagId, __, token) => HaveAtLeastOneNonVoidedRequirementAsync(tagId, token))
                    .WithMessage((_, id) => $"Tag do not have any non voided requirement! Tag={id}");
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

            async Task<bool> IsReadyToBeStartedAsync(int tagId, CancellationToken token)
                => await tagValidator.IsReadyToBeStartedAsync(tagId, token);

            async Task<bool> HaveAtLeastOneNonVoidedRequirementAsync(int tagId, CancellationToken token)
                => await tagValidator.HasANonVoidedRequirementAsync(tagId, token);
        }
    }
}
