using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.Project;
using Equinor.Procosys.Preservation.Command.Validators.Tag;
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

            When(tag => tag.TagIds.Any() && BeUniqueTags(tag.TagIds), () =>
            {
                RuleForEach(command => command.TagIds)
                    .MustAsync((_, tagId, __, token) => NotBeAClosedProjectForTagAsync(tagId, token))
                    .WithMessage((_, id) => $"Project for tag is closed! Tag={id}")
                    .Must(BeAnExistingTag)
                    .WithMessage((_, id) => $"Tag doesn't exists! Tag={id}")
                    .Must(NotBeAVoidedTag)
                    .WithMessage((_, id) => $"Tag is voided! Tag={id}")
                    .Must(PreservationIsStarted)
                    .WithMessage((_, id) => $"Tag must have status {PreservationStatus.Active} to transfer! Tag={id}")
                    .Must(HaveNextStep)
                    .WithMessage((_, id) => $"Tag doesn't have a next step to transfer to! Tag={id}");
            });

            bool BeUniqueTags(IEnumerable<int> tagIds)
            {
                var ids = tagIds.ToList();
                return ids.Distinct().Count() == ids.Count;
            }
            
            async Task<bool> NotBeAClosedProjectForTagAsync(int tagId, CancellationToken cancellationToken)
                => !await projectValidator.IsClosedForTagAsync(tagId, cancellationToken);

            bool BeAnExistingTag(int tagId) => tagValidator.Exists(tagId);

            bool NotBeAVoidedTag(int tagId) => !tagValidator.IsVoided(tagId);

            bool PreservationIsStarted(int tagId) => tagValidator.VerifyPreservationStatus(tagId, PreservationStatus.Active);
            
            bool HaveNextStep(int tagId) => tagValidator.HaveNextStep(tagId);
        }
    }
}
