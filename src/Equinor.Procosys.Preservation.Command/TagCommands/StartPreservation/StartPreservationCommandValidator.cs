using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.TagValidators;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
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
            CascadeMode = CascadeMode.StopOnFirstFailure;
                        
            RuleFor(command => command.TagIds)
                .Must(ids => ids != null && ids.Any())
                .WithMessage("At least 1 tag must be given!")
                .Must(BeUniqueTags)
                .WithMessage("Tags must be unique!");

            When(command => command.TagIds.Any() && BeUniqueTags(command.TagIds), () =>
            {
                RuleForEach(command => command.TagIds)
                    .MustAsync((_, tagId, __, token) => NotBeAClosedProjectForTagAsync(tagId, token))
                    .WithMessage((_, id) => $"Project for tag is closed! Tag={id}")
                    .MustAsync((_, tagId, __, token) => BeAnExistingTagAsync(tagId, token))
                    .WithMessage((_, id) => $"Tag doesn't exists! Tag={id}")
                    .MustAsync((_, tagId, __, token) => NotBeAVoidedTagAsync(tagId, token))
                    .WithMessage((_, id) => $"Tag is voided! Tag={id}")
                    .MustAsync((_, tagId, __, token) => PreservationIsNotStartedAsync(tagId, token))
                    .WithMessage((_, id) => $"Tag must have status {PreservationStatus.NotStarted} to start! Tag={id}")
                    .MustAsync((_, tagId, __, token) => HaveAtLeastOneNonVoidedRequirementAsync(tagId, token))
                    .WithMessage((_, id) => $"Tag do not have any non voided requirement! Tag={id}")
                    .MustAsync((_, tagId, __, token) => HaveExistingRequirementDefinitionsAsync(tagId, token))
                    .WithMessage((_, id) => $"A requirement definition doesn't exists! Tag={id}");
            });

            bool BeUniqueTags(IEnumerable<int> tagIds)
            {
                var ids = tagIds.ToList();
                return ids.Distinct().Count() == ids.Count;
            }
            
            async Task<bool> NotBeAClosedProjectForTagAsync(int tagId, CancellationToken token)
                => !await projectValidator.IsClosedForTagAsync(tagId, token);

            async Task<bool> BeAnExistingTagAsync(int tagId, CancellationToken token)
                => await tagValidator.ExistsAsync(tagId, token);

            async Task<bool> NotBeAVoidedTagAsync(int tagId, CancellationToken token)
                => !await tagValidator.IsVoidedAsync(tagId, token);

            async Task<bool> PreservationIsNotStartedAsync(int tagId, CancellationToken token)
                => await tagValidator.VerifyPreservationStatusAsync(tagId, PreservationStatus.NotStarted, token);

            async Task<bool> HaveAtLeastOneNonVoidedRequirementAsync(int tagId, CancellationToken token)
                => await tagValidator.HasANonVoidedRequirementAsync(tagId, token);
            
            async Task<bool> HaveExistingRequirementDefinitionsAsync(int tagId, CancellationToken token)
                => await tagValidator.AllRequirementDefinitionsExistAsync(tagId, token);
        }
    }
}
