﻿using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.AttachmentValidators;
using Equinor.Procosys.Preservation.Command.Validators.ProjectValidators;
using Equinor.Procosys.Preservation.Command.Validators.TagValidators;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.TagAttachmentCommands.Delete
{
    public class DeleteTagAttachmentCommandValidator : AbstractValidator<DeleteTagAttachmentCommand>
    {
        public DeleteTagAttachmentCommandValidator(
            IProjectValidator projectValidator,
            ITagValidator tagValidator,
            IAttachmentValidator attachmentValidator)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(command => command)
                .MustAsync((command, token) => NotBeAClosedProjectForTagAsync(command.TagId, token))
                .WithMessage(command => $"Project for tag is closed! Tag={command.TagId}")
                .MustAsync((command, token) => BeAnExistingTagAsync(command.TagId, token))
                .WithMessage(command => $"Tag doesn't exist! Tag={command.TagId}")
                .MustAsync((command, token) => NotBeAVoidedTagAsync(command.TagId, token))
                .WithMessage(command => $"Tag is voided! Tag={command.TagId}")
                .MustAsync((command, token) => BeAnExistingAttachmentAsync(command.AttachmentId, token))
                .WithMessage(command => $"Attachment doesn't exist! Attachment={command.AttachmentId}");

            async Task<bool> NotBeAClosedProjectForTagAsync(int tagId, CancellationToken token)
                => !await projectValidator.IsClosedForTagAsync(tagId, token);
            async Task<bool> BeAnExistingTagAsync(int tagId, CancellationToken token)
                => await tagValidator.ExistsAsync(tagId, token);
            async Task<bool> NotBeAVoidedTagAsync(int tagId, CancellationToken token)
                => !await tagValidator.IsVoidedAsync(tagId, token);
            async Task<bool> BeAnExistingAttachmentAsync(int attachmentId, CancellationToken token)
                => await attachmentValidator.ExistsAsync(attachmentId, token);
        }
    }
}
