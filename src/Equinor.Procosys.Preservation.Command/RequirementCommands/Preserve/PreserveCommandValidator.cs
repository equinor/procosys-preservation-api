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
                .MustAsync((command, token) => BeAnExistingTag(command.TagId, token))
                .WithMessage((x, id) => $"Tag doesn't exists! Tag={id}")
                .MustAsync((command, token) => NotBeAVoidedTag(command.TagId, token))
                .WithMessage((x, id) => $"Tag is voided! Tag={id}")
                .MustAsync((command, token) => PreservationIsStarted(command.TagId, token))
                .WithMessage((x, id) => $"Tag must have status {PreservationStatus.Active} to preserve! Tag={id}")
                .MustAsync((command, token) => HasRequirementReadyToBePreserved(command.TagId, command.RequirementId, token))
                .WithMessage((command, _) =>
                    $"Tag doesn't have this requirement ready to be preserved! Tag={command.TagId}. Requirement={command.RequirementId}");
            
            async Task<bool> NotBeAClosedProjectForTagAsync(int tagId, CancellationToken token)
                => !await projectValidator.IsClosedForTagAsync(tagId, token);

            async Task<bool> BeAnExistingTag(int tagId, CancellationToken token)
                => await tagValidator.ExistsAsync(tagId, token);

            async Task<bool> NotBeAVoidedTag(int tagId, CancellationToken token)
                => !await tagValidator.IsVoidedAsync(tagId, token);

            async Task<bool> PreservationIsStarted(int tagId, CancellationToken token)
                => await tagValidator.VerifyPreservationStatusAsync(tagId, PreservationStatus.Active, token);

            async Task<bool> HasRequirementReadyToBePreserved(int tagId, int requirementId, CancellationToken token)
                =>  await tagValidator.HasRequirementReadyToBePreservedAsync(tagId, requirementId, token);
        }
    }
}
