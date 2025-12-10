using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Validators;
using Equinor.ProCoSys.Preservation.Command.Validators.TagValidators;
using FluentValidation;

namespace Equinor.ProCoSys.Preservation.Command.TagCommands.DeleteTag
{
    public class DeleteTagCommandValidator : AbstractValidator<DeleteTagCommand>
    {

        public DeleteTagCommandValidator(
            ITagValidator tagValidator,
            IRowVersionValidator rowVersionValidator)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(command => command)
                .MustAsync((command, token) => BeAnExistingTagAsync(command.TagId, token))
                .WithMessage(command => $"Tag doesn't exist! TagId={command.TagId}")
                .MustAsync((command, token) => BeAVoidedTagAsync(command.TagId, token))
                .WithMessage(command => $"Tag is not voided! Tag='{GetTagDetails(command.TagId)}'")
                .MustAsync((command, token) => NotBeInUse(command.TagId, token))
                .WithMessage(command => $"Tag is in use! Tag='{GetTagDetails(command.TagId)}'")
                .Must(command => HaveAValidRowVersion(command.RowVersion))
                .WithMessage(command => $"Not a valid row version! Row version={command.RowVersion}");

            string GetTagDetails(int tagId) => tagValidator.GetTagDetailsAsync(tagId, default).Result;

            async Task<bool> BeAnExistingTagAsync(int tagId, CancellationToken token)
                => await tagValidator.ExistsAsync(tagId, token);

            async Task<bool> BeAVoidedTagAsync(int tagId, CancellationToken token)
                => await tagValidator.IsVoidedAsync(tagId, token);

            async Task<bool> NotBeInUse(int tagId, CancellationToken token)
                => !await tagValidator.IsInUseAsync(tagId, token);

            bool HaveAValidRowVersion(string rowVersion)
                => rowVersionValidator.IsValid(rowVersion);
        }
    }
}
