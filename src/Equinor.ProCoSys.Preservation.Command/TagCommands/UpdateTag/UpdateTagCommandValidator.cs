using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Validators;
using Equinor.ProCoSys.Preservation.Command.Validators.ProjectValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.TagValidators;
using FluentValidation;

namespace Equinor.ProCoSys.Preservation.Command.TagCommands.UpdateTag
{
    public class UpdateTagCommandValidator : AbstractValidator<UpdateTagCommand>
    {
        public UpdateTagCommandValidator(
             IProjectValidator projectValidator,
             ITagValidator tagValidator,
             IRowVersionValidator rowVersionValidator)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(command => command)
                .MustAsync((command, token) => NotBeAClosedProjectForTagAsync(command.TagId, token))
                .WithMessage(command => $"Project for tag is closed! Tag='{GetTagDetails(command.TagId)}'")
                .MustAsync((command, token) => BeAnExistingTagAsync(command.TagId, token))
                .WithMessage(command => $"Tag doesn't exist! TagId={command.TagId}")
                .MustAsync((command, token) => BeReadyForEditingAsync(command.TagId, token))
                .WithMessage(command => $"Tag can't be edited! Tag='{GetTagDetails(command.TagId)}'")
                .MustAsync((command, token) => NotBeAVoidedTagAsync(command.TagId, token))
                .WithMessage(command => $"Tag is voided! Tag='{GetTagDetails(command.TagId)}'")
                .Must(command => HaveAValidRowVersion(command.RowVersion))
                .WithMessage(command => $"Not a valid row version! Row version={command.RowVersion}");

            string GetTagDetails(int tagId) => tagValidator.GetTagDetailsAsync(tagId, default).Result;

            async Task<bool> NotBeAClosedProjectForTagAsync(int tagId, CancellationToken token)
                => !await projectValidator.IsClosedForTagAsync(tagId, token);

            async Task<bool> BeReadyForEditingAsync(int tagId, CancellationToken token)
                => await tagValidator.IsReadyToBeEditedAsync(tagId, token);

            async Task<bool> BeAnExistingTagAsync(int tagId, CancellationToken token)
                => await tagValidator.ExistsAsync(tagId, token);

            async Task<bool> NotBeAVoidedTagAsync(int tagId, CancellationToken token)
                => !await tagValidator.IsVoidedAsync(tagId, token);

            bool HaveAValidRowVersion(string rowVersion)
                => rowVersionValidator.IsValid(rowVersion);
        }
    }
}
