using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Command.Validators.Tag;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.TagCommands.Transfer
{
    public class TransferCommandValidator : AbstractValidator<TransferCommand>
    {
        public TransferCommandValidator(ITagValidator tagValidator)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
                        
            RuleFor(tag => tag.TagIds)
                .Must(r => r != null && r.Any())
                .WithMessage("At least 1 tag must be given!")
                .Must(BeUniqueTags)
                .WithMessage("Tags must be unique!");

            When(tag => tag.TagIds.Any() && BeUniqueTags(tag.TagIds), () =>
            {
                RuleForEach(s => s.TagIds)
                    .Must(BeAnExistingTag)
                    .WithMessage((x, id) => $"Tag doesn't exists! Tag={id}")
                    .Must(NotBeAVoidedTag)
                    .WithMessage((x, id) => $"Tag is voided! Tag={id}")
                    .Must(NotBeInAClosedProject)
                    .WithMessage((x, id) => $"Project for tag is closed! Tag={id}")
                    .Must(PreservationIsStarted)
                    .WithMessage((x, id) => $"Tag must have status {PreservationStatus.Active} to transfer! Tag={id}")
                    .Must(HaveNextStep)
                    .WithMessage((x, id) => $"Tag doesn't have a next step to transfer to! Tag={id}");
            });

            bool BeUniqueTags(IEnumerable<int> tagIds)
            {
                var ids = tagIds.ToList();
                return ids.Distinct().Count() == ids.Count;
            }

            bool BeAnExistingTag(int tagId) => tagValidator.Exists(tagId);

            bool NotBeAVoidedTag(int tagId) => !tagValidator.IsVoided(tagId);

            bool NotBeInAClosedProject(int tagId) => !tagValidator.ProjectIsClosed(tagId);

            bool PreservationIsStarted(int tagId) => tagValidator.VerifyPreservationStatus(tagId, PreservationStatus.Active);
            
            bool HaveNextStep(int tagId) => tagValidator.HaveNextStep(tagId);
        }
    }
}
