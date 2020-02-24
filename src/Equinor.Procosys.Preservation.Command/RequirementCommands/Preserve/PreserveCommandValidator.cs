using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.ProjectValidators;
using Equinor.Procosys.Preservation.Command.Validators.TagValidators;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.RequirementCommands.Preserve
{
    public class PreserveCommandValidator : AbstractValidator<PreserveCommand>
    {
        public PreserveCommandValidator(IProjectValidator projectValidator, ITagValidator tagValidator)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            
            RuleFor(command => command)
                .MustAsync((command, token) => NotBeAClosedProjectForTagAsync(command.TagId, token))
                .WithMessage(command => $"Project for tag is closed! Tag={command.TagId}")
                .Must(command => BeAnExistingTag(command.TagId))
                .WithMessage((x, id) => $"Tag doesn't exists! Tag={id}")
                .Must(command => NotBeAVoidedTag(command.TagId))
                .WithMessage((x, id) => $"Tag is voided! Tag={id}")
                .Must(command => PreservationIsStarted(command.TagId))
                .WithMessage((x, id) => $"Tag must have status {PreservationStatus.Active} to preserve! Tag={id}")
                .Must((command, _) => HaveRequirementReadyToBePreserved(command.TagId, command.RequirementId))
                .WithMessage((command, _) =>
                    $"Tag doesn't have this requirement ready to be preserved! Tag={command.TagId}. Requirement={command.RequirementId}");
            
            async Task<bool> NotBeAClosedProjectForTagAsync(int tagId, CancellationToken cancellationToken)
                => !await projectValidator.IsClosedForTagAsync(tagId, cancellationToken);

            bool BeAnExistingTag(int tagId) => tagValidator.ExistsAsync(tagId);

            bool NotBeAVoidedTag(int tagId) => !tagValidator.IsVoidedAsync(tagId);

            bool PreservationIsStarted(int tagId) => tagValidator.VerifyPreservationStatusAsync(tagId, PreservationStatus.Active);

            bool HaveRequirementReadyToBePreserved(int tagId, int requirementId)
                => tagValidator.HaveRequirementReadyToBePreservedAsync(tagId, requirementId);
        }
    }
}
