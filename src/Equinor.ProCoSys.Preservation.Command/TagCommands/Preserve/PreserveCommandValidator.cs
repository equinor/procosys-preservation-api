using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Validators.ProjectValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.TagValidators;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using FluentValidation;

namespace Equinor.ProCoSys.Preservation.Command.TagCommands.Preserve
{
    public class PreserveCommandValidator : AbstractValidator<PreserveCommand>
    {
        public PreserveCommandValidator(
            IProjectValidator projectValidator,
            ITagValidator tagValidator)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            
            RuleFor(command => command)
                .MustAsync((command, token) => NotBeAClosedProjectForTagAsync(command.TagId, token))
                .WithMessage(command => $"Project for tag is closed! Tag='{GetTagDetails(command.TagId)}'")
                .MustAsync((command, token) => BeAnExistingTagAsync(command.TagId, token))
                .WithMessage(command => $"Tag doesn't exist! TagId={command.TagId}")
                .MustAsync((command, token) => NotBeAVoidedTag(command.TagId, token))
                .WithMessage(command => $"Tag is voided! Tag='{GetTagDetails(command.TagId)}'")
                .MustAsync((command, token) => PreservationIsStartedAsync(command.TagId, token))
                .WithMessage(command => $"Tag must have status {PreservationStatus.Active} to preserve! Tag='{GetTagDetails(command.TagId)}'")
                .MustAsync((command, token) => BeReadyToBePreservedAsync(command.TagId, token))
                .WithMessage(command => $"Tag is not ready to be preserved! Tag='{GetTagDetails(command.TagId)}'");

            string GetTagDetails(int tagId) => tagValidator.GetTagDetailsAsync(tagId, default).Result;

            async Task<bool> NotBeAClosedProjectForTagAsync(int tagId, CancellationToken token)
                => !await projectValidator.IsClosedForTagAsync(tagId, token);

            async Task<bool> BeAnExistingTagAsync(int tagId, CancellationToken token)
                => await tagValidator.ExistsAsync(tagId, token);

            async Task<bool> NotBeAVoidedTag(int tagId, CancellationToken token)
                => !await tagValidator.IsVoidedAsync(tagId, token);

            async Task<bool> PreservationIsStartedAsync(int tagId, CancellationToken token)
                => await tagValidator.VerifyPreservationStatusAsync(tagId, PreservationStatus.Active, token);

            async Task<bool> BeReadyToBePreservedAsync(int tagId, CancellationToken token)
                => await tagValidator.IsReadyToBePreservedAsync(tagId, token);
        }
    }
}
