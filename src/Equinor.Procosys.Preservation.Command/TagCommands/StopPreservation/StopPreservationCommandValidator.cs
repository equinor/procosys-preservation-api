using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.TagValidators;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Command.Validators.ProjectValidators;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.TagCommands.StopPreservation
{
    public class StopPreservationCommandValidator : AbstractValidator<StopPreservationCommand>
    {
        public StopPreservationCommandValidator(
            IProjectValidator projectValidator,
            ITagValidator tagValidator)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
                        
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
                    .WithMessage((_, id) => $"Tag doesn't exists! Tag={id}")
                    .MustAsync((_, tagId, __, token) => NotBeAVoidedTagAsync(tagId, token))
                    .WithMessage((_, id) => $"Tag is voided! Tag={id}")
                    .MustAsync((_, tagId, __, token) => PreservationIsActiveAsync(tagId, token))
                    .WithMessage((_, id) => $"Tag must have status {PreservationStatus.Active} to be able to stop! Tag={id}")
                    .MustAsync((_, tagId, __, token) => CanBeStoppedAsync(tagId, token))
                    .WithMessage((_, id) => $"{TagType.Standard} and {TagType.PreArea} tags must be in last step of journey to be able to stop! Tag={id}")
                    .MustAsync((_, tagId, __, token) => HaveExistingRequirementDefinitionsAsync(tagId, token))
                    .WithMessage((_, id) => $"A requirement definition doesn't exists! Tag={id}");
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

            async Task<bool> PreservationIsActiveAsync(int tagId, CancellationToken token)
                => await tagValidator.VerifyPreservationStatusAsync(tagId, PreservationStatus.Active, token);

            async Task<bool> HaveExistingRequirementDefinitionsAsync(int tagId, CancellationToken token)
                => await tagValidator.AllRequirementDefinitionsExistAsync(tagId, token);

            async Task<bool> CanBeStoppedAsync(int tagId, CancellationToken token)
            {
                var tagFollowsAJourney = await tagValidator.TagFollowsAJourneyAsync(tagId, token);
                return !tagFollowsAJourney || !await tagValidator.HaveNextStepAsync(tagId, token);
            }
        }
    }
}
