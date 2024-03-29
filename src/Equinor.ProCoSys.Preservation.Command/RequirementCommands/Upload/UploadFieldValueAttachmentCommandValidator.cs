﻿using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Validators.FieldValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.ProjectValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.TagValidators;
using FluentValidation;

namespace Equinor.ProCoSys.Preservation.Command.RequirementCommands.Upload
{
    public class UploadFieldValueAttachmentCommandValidator : AbstractValidator<UploadFieldValueAttachmentCommand>
    {
        public UploadFieldValueAttachmentCommandValidator(
            IProjectValidator projectValidator,
            ITagValidator tagValidator,
            IFieldValidator fieldValidator)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(command => command)
                .MustAsync((command, token) => NotBeAClosedProjectForTagAsync(command.TagId, token))
                .WithMessage(command => $"Project for tag is closed! Tag={command.TagId}")
                .MustAsync(BeAnExistingRequirementAsync)
                .WithMessage(_ => "Tag and/or requirement doesn't exist!")
                .MustAsync(BeAnExistingFieldForRequirementAsync)
                .WithMessage(_ => "Field doesn't exist in requirement!")
                .MustAsync((command, token) => NotBeAVoidedTagAsync(command.TagId, token))
                .WithMessage(command => $"Tag is voided! Tag={command.TagId}")
                .MustAsync((command, token) => HasRequirementWithActivePeriodAsync(command.TagId, command.RequirementId, token))
                .WithMessage(command =>
                    $"Tag doesn't have this requirement with active period! Tag={command.TagId}. Requirement={command.RequirementId}")
                .MustAsync((command, token) => BeAFieldForAttachmentAsync(command.FieldId, token))
                .WithMessage(command => $"Field values can not be recorded for field type! Field={command.FieldId}")
                .MustAsync((command, token) => NotBeAVoidedFieldAsync(command.FieldId, token))
                .WithMessage(command => $"Field is voided! Field={command.FieldId}");
                        
            async Task<bool> NotBeAClosedProjectForTagAsync(int tagId, CancellationToken token)
                => !await projectValidator.IsClosedForTagAsync(tagId, token);

            async Task<bool> BeAnExistingRequirementAsync(UploadFieldValueAttachmentCommand command, CancellationToken token)
                => await tagValidator.ExistsRequirementAsync(command.TagId, command.RequirementId, token);

            async Task<bool> BeAnExistingFieldForRequirementAsync(UploadFieldValueAttachmentCommand command, CancellationToken token)
                => await tagValidator.ExistsFieldForRequirementAsync(command.TagId, command.RequirementId, command.FieldId, token);

            async Task<bool> NotBeAVoidedTagAsync(int tagId, CancellationToken token)
                => !await tagValidator.IsVoidedAsync(tagId, token);

            async Task<bool> HasRequirementWithActivePeriodAsync(int tagId, int requirementId, CancellationToken token)
                => await tagValidator.HasRequirementWithActivePeriodAsync(tagId, requirementId, token);

            async Task<bool> NotBeAVoidedFieldAsync(int fieldId, CancellationToken token)
                => !await fieldValidator.IsVoidedAsync(fieldId, token);

            async Task<bool> BeAFieldForAttachmentAsync(int fieldId, CancellationToken token)
                => await fieldValidator.IsValidForAttachmentAsync(fieldId, token);
        }
    }
}
