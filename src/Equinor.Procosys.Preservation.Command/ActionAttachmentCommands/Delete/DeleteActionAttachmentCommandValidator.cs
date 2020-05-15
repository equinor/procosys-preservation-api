using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.ActionValidators;
using Equinor.Procosys.Preservation.Command.Validators.AttachmentValidators;
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
            IAttachmentValidator attachmentValidator,
            IActionValidator actionValidator)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(command => command)
                .MustAsync((command, token) => NotBeAClosedProjectForTagAsync(command.TagId, token))
                .WithMessage(command => $"Project for tag is closed! Tag={command.TagId}")
                .MustAsync((command, token) => BeAnExistingTagAsync(command.TagId, token))
                .WithMessage(command => $"Tag doesn't exist! Tag={command.TagId}")
                .MustAsync((command, token) => NotBeAVoidedTagAsync(command.TagId, token))
                .WithMessage(command => $"Tag is voided! Tag={command.TagId}")
                .MustAsync((command, token) => BeAnExistingActionAsync(command.ActionId, token))
                .WithMessage(command => $"Action doesn't exist! Action={command.ActionId}")
                .MustAsync((command, token) => NotBeAClosedActionAsync(command.ActionId, token))
                .WithMessage(command => $"Action is closed! Action={command.ActionId}")
                .MustAsync((command, token) => BeAnExistingAttachmentAsync(command.AttachmentId, token))
                .WithMessage(command => $"Attachment doesn't exist! Attachment={command.AttachmentId}");

            async Task<bool> NotBeAClosedProjectForTagAsync(int tagId, CancellationToken token)
                => !await projectValidator.IsClosedForTagAsync(tagId, token);
            async Task<bool> BeAnExistingTagAsync(int tagId, CancellationToken token)
                => await tagValidator.ExistsAsync(tagId, token);
            async Task<bool> NotBeAVoidedTagAsync(int tagId, CancellationToken token)
                => !await tagValidator.IsVoidedAsync(tagId, token);
            async Task<bool> BeAnExistingActionAsync(int actionId, CancellationToken token)
                => await actionValidator.ExistsAsync(actionId, token);
            async Task<bool> NotBeAClosedActionAsync(int actionId, CancellationToken token)
                => !await actionValidator.IsClosedAsync(actionId, token);
            async Task<bool> BeAnExistingAttachmentAsync(int attachmentId, CancellationToken token)
                => await attachmentValidator.ExistsAsync(attachmentId, token);
        }
    }
}
