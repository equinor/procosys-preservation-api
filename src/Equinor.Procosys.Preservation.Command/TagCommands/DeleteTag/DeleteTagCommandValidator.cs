using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators;
using Equinor.Procosys.Preservation.Command.Validators.TagValidators;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.TagCommands.DeleteTag
{
    public class DeleteTagCommandValidator : AbstractValidator<DeleteTagCommand>
    {

        public DeleteTagCommandValidator(
            ITagValidator tagValidator,
            IRowVersionValidator rowVersionValidator)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(command => command)
                .MustAsync((command, token) => BeAnExistingTagAsync(command.TagId, token))
                .WithMessage(command => $"Tag doesn't exist! Tag={command.TagId}")
                .MustAsync((command, token) => BeAVoidedTagAsync(command.TagId, token))
                .WithMessage(command => $"Tag is not voided! Tag={command.TagId}")
                .MustAsync((command, token) => NotBeInUse(command.TagId, token))
                .WithMessage(command => $"Tag is in use! Tag={command.TagId}")
                .MustAsync((command, token) => HaveAValidRowVersion(command.RowVersion, token))
                .WithMessage(command => $"Not a valid RowVersion! RowVersion={command.RowVersion}");

            async Task<bool> BeAnExistingTagAsync(int tagId, CancellationToken token)
                => await tagValidator.ExistsAsync(tagId, token);
            async Task<bool> BeAVoidedTagAsync(int tagId, CancellationToken token)
                => await tagValidator.IsVoidedAsync(tagId, token);
            async Task<bool> NotBeInUse(int tagId, CancellationToken token)
                => !await tagValidator.IsInUseAsync(tagId, token);
            async Task<bool> HaveAValidRowVersion(string rowVersion, CancellationToken token)
                => await rowVersionValidator.IsValid(rowVersion, token);
        }
    }
}
