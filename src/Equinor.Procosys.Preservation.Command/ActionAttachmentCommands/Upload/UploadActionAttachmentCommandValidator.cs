using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Validators.ActionValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.ProjectValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.TagValidators;
using FluentValidation;

namespace Equinor.ProCoSys.Preservation.Command.ActionAttachmentCommands.Upload
{
    public class UploadActionAttachmentCommandValidator : AbstractValidator<UploadActionAttachmentCommand>
    {
        public UploadActionAttachmentCommandValidator(
            IProjectValidator projectValidator,
            ITagValidator tagValidator,
            IActionValidator actionValidator)
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(command => command)
                .MustAsync((command, token) => NotBeAClosedProjectForTagAsync(command.TagId, token))
                .WithMessage(command => $"Project for tag is closed! Tag={command.TagId}")
                .MustAsync((command, token) => NotBeAVoidedTagAsync(command.TagId, token))
                .WithMessage(command => $"Tag is voided! Tag={command.TagId}")
                .MustAsync(BeAnExistingActionAsync)
                .WithMessage(command => "Tag and/or action doesn't exist!")
                .MustAsync((command, token) => NotBeAClosedActionAsync(command.ActionId, token))
                .WithMessage(command => $"Action is closed! Action={command.ActionId}")
                .MustAsync((command, token) => NotHaveAttachmentWithFilenameAsync(command.ActionId, command.FileName, token))
                .WithMessage(command => $"Action already has an attachment with filename {command.FileName}! Please rename file or choose to overwrite")
                    .When(c => !c.OverwriteIfExists, ApplyConditionTo.CurrentValidator);

            async Task<bool> NotBeAClosedProjectForTagAsync(int tagId, CancellationToken token)
                => !await projectValidator.IsClosedForTagAsync(tagId, token);
            async Task<bool> NotBeAVoidedTagAsync(int tagId, CancellationToken token)
                => !await tagValidator.IsVoidedAsync(tagId, token);
            async Task<bool> NotHaveAttachmentWithFilenameAsync(int actionId, string fileName, CancellationToken token)
                => !await actionValidator.AttachmentWithFilenameExistsAsync(actionId, fileName, token);
            async Task<bool> BeAnExistingActionAsync(UploadActionAttachmentCommand command, CancellationToken token)
                => await tagValidator.ExistsActionAsync(command.TagId, command.ActionId, token);
            async Task<bool> NotBeAClosedActionAsync(int actionId, CancellationToken token)
                => !await actionValidator.IsClosedAsync(actionId, token);
        }
    }
}
