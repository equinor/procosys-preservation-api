using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Validators.ProjectValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.TagValidators;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using FluentValidation;

namespace Equinor.ProCoSys.Preservation.Command.RequirementCommands.Preserve
{
    public class PreserveCommandValidator : AbstractValidator<PreserveCommand>
    {
        public PreserveCommandValidator(IProjectValidator projectValidator, ITagValidator tagValidator)
        {
            CascadeMode = CascadeMode.Stop;
            
            RuleFor(command => command)
                .MustAsync((command, token) => NotBeAClosedProjectForTagAsync(command.TagId, token))
                .WithMessage(command => $"Project for tag is closed! Tag={command.TagId}")
                .MustAsync(BeAnExistingRequirementAsync)
                .WithMessage((x, id) => "Tag and/or requirement doesn't exist!")
                .MustAsync((command, token) => NotBeAVoidedTagAsync(command.TagId, token))
                .WithMessage((x, id) => $"Tag is voided! Tag={id}")
                .MustAsync((command, token) => PreservationIsStartedAsync(command.TagId, token))
                .WithMessage((x, id) => $"Tag must have status {PreservationStatus.Active} to preserve! Tag={id}")
                .MustAsync((command, token) => RequirementIsReadyToBePreservedAsync(command.TagId, command.RequirementId, token))
                .WithMessage((command, _) =>
                    $"Tag doesn't have this requirement ready to be preserved! Tag={command.TagId}. Requirement={command.RequirementId}");
            
            async Task<bool> NotBeAClosedProjectForTagAsync(int tagId, CancellationToken token)
                => !await projectValidator.IsClosedForTagAsync(tagId, token);

            async Task<bool> BeAnExistingRequirementAsync(PreserveCommand command, CancellationToken token)
                => await tagValidator.ExistsRequirementAsync(command.TagId, command.RequirementId, token);

            async Task<bool> NotBeAVoidedTagAsync(int tagId, CancellationToken token)
                => !await tagValidator.IsVoidedAsync(tagId, token);

            async Task<bool> PreservationIsStartedAsync(int tagId, CancellationToken token)
                => await tagValidator.VerifyPreservationStatusAsync(tagId, PreservationStatus.Active, token);

            async Task<bool> RequirementIsReadyToBePreservedAsync(int tagId, int requirementId, CancellationToken token)
                =>  await tagValidator.RequirementIsReadyToBePreservedAsync(tagId, requirementId, token);
        }
    }
}
