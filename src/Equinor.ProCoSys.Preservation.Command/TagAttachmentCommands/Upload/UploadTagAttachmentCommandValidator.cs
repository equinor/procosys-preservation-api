﻿using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Validators.ProjectValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.TagValidators;
using FluentValidation;

namespace Equinor.ProCoSys.Preservation.Command.TagAttachmentCommands.Upload
{
    public class UploadTagAttachmentCommandValidator : AbstractValidator<UploadTagAttachmentCommand>
    {
        public UploadTagAttachmentCommandValidator(
            IProjectValidator projectValidator,
            ITagValidator tagValidator)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(command => command)
                .MustAsync((command, token) => NotBeAClosedProjectForTagAsync(command.TagId, token))
                .WithMessage(command => $"Project for tag is closed! Tag={command.TagId}")
                .MustAsync((command, token) => BeAnExistingTagAsync(command.TagId, token))
                .WithMessage(command => $"Tag doesn't exist! Tag={command.TagId}")
                .MustAsync((command, token) => NotBeAVoidedTagAsync(command.TagId, token))
                .WithMessage(command => $"Tag is voided! Tag={command.TagId}")
                .MustAsync((command, token) => NotHaveAttachmentWithFilenameAsync(command.TagId, command.FileName, token))
                .WithMessage(command => $"Tag already has an attachment with filename {command.FileName}! Please rename file or choose to overwrite")
                    .When(c => !c.OverwriteIfExists, ApplyConditionTo.CurrentValidator);

            async Task<bool> NotBeAClosedProjectForTagAsync(int tagId, CancellationToken token)
                => !await projectValidator.IsClosedForTagAsync(tagId, token);
            async Task<bool> BeAnExistingTagAsync(int tagId, CancellationToken token)
                => await tagValidator.ExistsAsync(tagId, token);
            async Task<bool> NotBeAVoidedTagAsync(int tagId, CancellationToken token)
                => !await tagValidator.IsVoidedAsync(tagId, token);
            async Task<bool> NotHaveAttachmentWithFilenameAsync(int tagId, string fileName, CancellationToken token)
                => !await tagValidator.AttachmentWithFilenameExistsAsync(tagId, fileName, token);
        }
    }
}
