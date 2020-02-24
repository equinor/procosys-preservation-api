using Equinor.Procosys.Preservation.Command.Validators.Tag;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.RequirementCommands.Preserve
{
    public class PreserveCommandValidator : AbstractValidator<PreserveCommand>
    {
        public PreserveCommandValidator(ITagValidator tagValidator)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            
            RuleFor(s => s.TagId)
                .Must(BeAnExistingTag)
                .WithMessage((x, id) => $"Tag doesn't exists! Tag={id}")
                .Must(NotBeAVoidedTag)
                .WithMessage((x, id) => $"Tag is voided! Tag={id}")
                .Must(NotBeInAClosedProject)
                .WithMessage((x, id) => $"Project for tag is closed! Tag={id}")
                .Must(PreservationIsStarted)
                .WithMessage((x, id) => $"Tag must have status {PreservationStatus.Active} to preserve! Tag={id}")
                .Must((command, _) => HaveRequirementReadyToBePreserved(command.TagId, command.RequirementId))
                .WithMessage((command, _) =>
                    $"Tag doesn't have this requirement ready to be preserved! Tag={command.TagId}. Requirement={command.RequirementId}");

            bool BeAnExistingTag(int tagId) => tagValidator.Exists(tagId);

            bool NotBeAVoidedTag(int tagId) => !tagValidator.IsVoided(tagId);

            bool NotBeInAClosedProject(int tagId) => !tagValidator.ProjectIsClosed(tagId);

            bool PreservationIsStarted(int tagId) => tagValidator.VerifyPreservationStatus(tagId, PreservationStatus.Active);

            bool HaveRequirementReadyToBePreserved(int tagId, int requirementId)
                => tagValidator.HaveRequirementReadyToBePreserved(tagId, requirementId);
        }
    }
}
