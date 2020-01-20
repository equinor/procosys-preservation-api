using Equinor.Procosys.Preservation.Command.Validators.Tag;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.TagCommands.StartPreservation
{
    public class StartPreservationCommandValidator : AbstractValidator<StartPreservationCommand>
    {
        public StartPreservationCommandValidator(ITagValidator tagValidator)
        {
            RuleForEach(s => s.TagIds)
                .Must(BeAnExistingTag)
                .WithMessage((x, id) => $"Tag doesn't exists! Tag={id}");

            RuleForEach(x => x.TagIds)
                .Must(NotBeAVoidedTag)
                .WithMessage((x, id) => $"Tag is voided! Tag={id}");
            
            RuleForEach(x => x.TagIds)
                .Must(NotBeInAClosedProject)
                .WithMessage((x, id) => $"Project for tag is closed! Tag={id}");
            
            RuleForEach(x => x.TagIds)
                .Must(PreservationIsNotStarted)
                .WithMessage((x, id) => $"Preservation is already started! Tag={id}");
            
            RuleForEach(x => x.TagIds)
                .Must(HaveAtLeastOneNonVoidedRequirement)
                .WithMessage((x, id) => $"Tag do not have any non voided requirement! Tag={id}");
            
            RuleForEach(x => x.TagIds)
                .Must(HaveExistingRequirementDefinitions)
                .WithMessage((x, id) => $"A requirement definition doesn't exists! Tag={id}");

            bool BeAnExistingTag(int tagId) => tagValidator.Exists(tagId);

            bool NotBeAVoidedTag(int tagId) => !tagValidator.IsVoided(tagId);

            bool NotBeInAClosedProject(int tagId) => !tagValidator.ProjectIsClosed(tagId);

            bool PreservationIsNotStarted(int tagId) => tagValidator.VerifyPreservationStatus(tagId, PreservationStatus.NotStarted);

            bool HaveAtLeastOneNonVoidedRequirement(int tagId) => tagValidator.HasANonVoidedRequirement(tagId);
            
            bool HaveExistingRequirementDefinitions(int tagId) => tagValidator.AllRequirementDefinitionsExists(tagId);
        }
    }
}
