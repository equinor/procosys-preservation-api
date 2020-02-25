using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.ProjectValidators;
using Equinor.Procosys.Preservation.Command.Validators.TagValidators;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.TagCommands.Preserve
{
    public class PreserveCommandValidator : AbstractValidator<PreserveCommand>
    {
        public PreserveCommandValidator(
            IProjectValidator projectValidator,
            ITagValidator tagValidator,
            ITimeService timeService)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            
            RuleFor(command => command)
                .MustAsync((command, token) => NotBeAClosedProjectForTagAsync(command.TagId, token))
                .WithMessage(command => $"Project for tag is closed! Tag={command.TagId}")
                .MustAsync((command, token) => BeAnExistingTag(command.TagId, token))
                .WithMessage(command => $"Tag doesn't exists! Tag={command.TagId}")
                .MustAsync((command, token) => NotBeAVoidedTag(command.TagId, token))
                .WithMessage(command => $"Tag is voided! Tag={command.TagId}")
                .MustAsync((command, token) => PreservationIsStarted(command.TagId, token))
                .WithMessage(command => $"Tag must have status {PreservationStatus.Active} to preserve! Tag={command.TagId}")
                .MustAsync((command, token) => BeReadyToBePreserved(command.TagId, token))
                .WithMessage(command => $"Tag is not ready to be preserved! Tag={command.TagId}");
            
            async Task<bool> NotBeAClosedProjectForTagAsync(int tagId, CancellationToken token)
                => !await projectValidator.IsClosedForTagAsync(tagId, token);

            async Task<bool> BeAnExistingTag(int tagId, CancellationToken token)
                => await tagValidator.ExistsAsync(tagId, token);

            async Task<bool> NotBeAVoidedTag(int tagId, CancellationToken token)
                => !await tagValidator.IsVoidedAsync(tagId, token);

            async Task<bool> PreservationIsStarted(int tagId, CancellationToken token)
                => await tagValidator.VerifyPreservationStatusAsync(tagId, PreservationStatus.Active, token);

            async Task<bool> BeReadyToBePreserved(int tagId, CancellationToken token)
                => await tagValidator.ReadyToBePreservedAsync(tagId, timeService.GetCurrentTimeUtc(), token);
        }
    }
}
