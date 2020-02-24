using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.Project;
using Equinor.Procosys.Preservation.Command.Validators.Tag;
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
                .Must(command => BeAnExistingTag(command.TagId))
                .WithMessage(command => $"Tag doesn't exists! Tag={command.TagId}")
                .Must(command => NotBeAVoidedTag(command.TagId))
                .WithMessage(command => $"Tag is voided! Tag={command.TagId}")
                .Must(command => PreservationIsStarted(command.TagId))
                .WithMessage(command => $"Tag must have status {PreservationStatus.Active} to preserve! Tag={command.TagId}")
                .Must(command => BeReadyToBePreserved(command.TagId))
                .WithMessage(command => $"Tag is not ready to be preserved! Tag={command.TagId}");
            
            async Task<bool> NotBeAClosedProjectForTagAsync(int tagId, CancellationToken cancellationToken)
                => !await projectValidator.IsClosedForTagAsync(tagId, cancellationToken);

            bool BeAnExistingTag(int tagId) => tagValidator.Exists(tagId);

            bool NotBeAVoidedTag(int tagId) => !tagValidator.IsVoided(tagId);

            bool PreservationIsStarted(int tagId) => tagValidator.VerifyPreservationStatus(tagId, PreservationStatus.Active);

            bool BeReadyToBePreserved(int tagId) => tagValidator.ReadyToBePreserved(tagId, timeService.GetCurrentTimeUtc());
        }
    }
}
