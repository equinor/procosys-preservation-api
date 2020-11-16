using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators;
using Equinor.Procosys.Preservation.Command.Validators.ActionValidators;
using Equinor.Procosys.Preservation.Command.Validators.ProjectValidators;
using Equinor.Procosys.Preservation.Command.Validators.TagValidators;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.ActionAttachmentCommands.Delete
{
    public class DeleteActionAttachmentCommandValidator : AbstractValidator<DeleteActionAttachmentCommand>
    {
        public DeleteActionAttachmentCommandValidator(
            IProjectValidator projectValidator,
            ITagValidator tagValidator,
            IActionValidator actionValidator,
            IRowVersionValidator rowVersionValidator)
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(command => command)
                .MustAsync((command, token) => NotBeAClosedProjectForTagAsync(command.TagId, token))
                .WithMessage(command => $"Project for tag is closed! Tag={command.TagId}")
                .MustAsync(BeAnExistingActionAttachmentAsync)
                .WithMessage(command => "Tag, action and/or attachment doesn't exist!")
                .MustAsync((command, token) => NotBeAVoidedTagAsync(command.TagId, token))
                .WithMessage(command => $"Tag is voided! Tag={command.TagId}")
                .MustAsync((command, token) => NotBeAClosedActionAsync(command.ActionId, token))
                .WithMessage(command => $"Action is closed! Action={command.ActionId}")
                .Must(command => HaveAValidRowVersion(command.RowVersion))
                .WithMessage(command => $"Not a valid row version! Row version={command.RowVersion}");

            async Task<bool> NotBeAClosedProjectForTagAsync(int tagId, CancellationToken token)
                => !await projectValidator.IsClosedForTagAsync(tagId, token);
            async Task<bool> BeAnExistingActionAttachmentAsync(DeleteActionAttachmentCommand command, CancellationToken token)
                => await tagValidator.ExistsActionAttachmentAsync(command.TagId, command.ActionId, command.AttachmentId, token);
            async Task<bool> NotBeAVoidedTagAsync(int tagId, CancellationToken token)
                => !await tagValidator.IsVoidedAsync(tagId, token);
            async Task<bool> NotBeAClosedActionAsync(int actionId, CancellationToken token)
                => !await actionValidator.IsClosedAsync(actionId, token);
            bool HaveAValidRowVersion(string rowVersion)
                => rowVersionValidator.IsValid(rowVersion);
        }
    }
}
