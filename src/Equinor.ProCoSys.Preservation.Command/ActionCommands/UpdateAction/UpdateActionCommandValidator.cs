using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Validators;
using Equinor.ProCoSys.Preservation.Command.Validators.ActionValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.ProjectValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.TagValidators;
using FluentValidation;

namespace Equinor.ProCoSys.Preservation.Command.ActionCommands.UpdateAction
{
    public class UpdateActionCommandValidator : AbstractValidator<UpdateActionCommand>
    {
        public UpdateActionCommandValidator(
            IProjectValidator projectValidator,
            ITagValidator tagValidator,
            IActionValidator actionValidator,
            IRowVersionValidator rowVersionValidator)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(command => command)
                .MustAsync((command, token) => NotBeAClosedProjectForTagAsync(command.TagId, token))
                .WithMessage(command => $"Project for tag is closed! Tag={command.TagId}")
                .MustAsync((command, token) => NotBeAVoidedTagAsync(command.TagId, token))
                .WithMessage(command => $"Tag is voided! Tag={command.TagId}")
                .MustAsync(BeAnExistingActionAsync)
                .WithMessage(_ => "Tag and/or action doesn't exist!")
                .MustAsync((command, token) => NotBeAClosedActionAsync(command.ActionId, token))
                .WithMessage(command => $"Action is closed! Action={command.ActionId}")
                .Must(command => HaveAValidRowVersion(command.RowVersion))
                .WithMessage(command => $"Not a valid row version! Row version={command.RowVersion}");

            async Task<bool> NotBeAClosedProjectForTagAsync(int tagId, CancellationToken token)
                => !await projectValidator.IsClosedForTagAsync(tagId, token);
            async Task<bool> NotBeAVoidedTagAsync(int tagId, CancellationToken token)
                => !await tagValidator.IsVoidedAsync(tagId, token);
            async Task<bool> BeAnExistingActionAsync(UpdateActionCommand command, CancellationToken token)
                => await tagValidator.ExistsActionAsync(command.TagId, command.ActionId, token);
            async Task<bool> NotBeAClosedActionAsync(int actionId, CancellationToken token)
                => !await actionValidator.IsClosedAsync(actionId, token);
            bool HaveAValidRowVersion(string rowVersion)
                => rowVersionValidator.IsValid(rowVersion);
        }
    }
}
