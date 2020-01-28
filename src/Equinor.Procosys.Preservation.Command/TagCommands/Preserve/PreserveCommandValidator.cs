using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Command.Validators.Tag;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.TagCommands.Preserve
{
    public class PreserveCommandValidator : AbstractValidator<PreserveCommand>
    {
        public PreserveCommandValidator(ITagValidator tagValidator)
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
                    .WithMessage((x, id) => $"Tag must have status {PreservationStatus.Active} to preserve! Tag={id}")
                    .Must(HaveAtLeastOneNonVoidedRequirement)
                    .WithMessage((x, id) => $"Tag do not have any non voided requirement! Tag={id}")
                    .Must(HaveExistingRequirementDefinitions)
                    .WithMessage((x, id) => $"A requirement definition doesn't exists! Tag={id}")
                    .Must(HaveRequirementsReadyToBePreserved)
                    .WithMessage((x, id) => $"A requirement is not ready to be preserved! Tag={id}")
                    ;
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

            bool HaveAtLeastOneNonVoidedRequirement(int tagId) => tagValidator.HasANonVoidedRequirement(tagId);
            
            bool HaveExistingRequirementDefinitions(int tagId) => tagValidator.AllRequirementDefinitionsExist(tagId);
            
            bool HaveRequirementsReadyToBePreserved(int tagId) => tagValidator.AllRequirementsReadyToBePreserved(tagId);
        }
    }
}
