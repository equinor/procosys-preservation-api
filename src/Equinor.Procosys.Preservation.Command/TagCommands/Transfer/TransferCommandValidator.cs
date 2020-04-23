using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.ProjectValidators;
using Equinor.Procosys.Preservation.Command.Validators.TagValidators;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.TagCommands.Transfer
{
    public class TransferCommandValidator : AbstractValidator<TransferCommand>
    {
        public TransferCommandValidator(
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
                    .WithMessage((_, id) => $"Tag doesn't exist! Tag={id}")
                    .MustAsync((_, tagId, __, token) => NotBeAVoidedTagAsync(tagId, token))
                    .WithMessage((_, id) => $"Tag is voided! Tag={id}")
                    .MustAsync((_, tagId, __, token) => PreservationIsStartedAsync(tagId, token))
                    .WithMessage((_, id) => $"Tag must have status {PreservationStatus.Active} to transfer! Tag={id}")
                    .MustAsync((_, tagId, __, token) => TagTypeCanBeTransferredAsync(tagId, token))
                    .WithMessage((_, id) => $"Tags of this type can not be transferred! Tag={id}")
                    .MustAsync((_, tagId, __, token) => HaveNextStepAsync(tagId, token))
                    .WithMessage((_, id) => $"Tag doesn't have a next step to transfer to! Tag={id}");
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
                => ! await tagValidator.IsVoidedAsync(tagId, token);

            async Task<bool> PreservationIsStartedAsync(int tagId, CancellationToken token)
                => await tagValidator.VerifyPreservationStatusAsync(tagId, PreservationStatus.Active, token);

            async Task<bool> TagTypeCanBeTransferredAsync(int tagId, CancellationToken token)
                => await tagValidator.TagFollowsAJourneyAsync(tagId, token);
            
            async Task<bool> HaveNextStepAsync(int tagId, CancellationToken token)
                => await tagValidator.HaveNextStepAsync(tagId, token);
        }
    }
}
