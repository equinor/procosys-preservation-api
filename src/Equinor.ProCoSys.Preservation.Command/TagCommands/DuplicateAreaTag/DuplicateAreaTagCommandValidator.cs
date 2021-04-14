using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Validators.ProjectValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.TagValidators;
using FluentValidation;

namespace Equinor.ProCoSys.Preservation.Command.TagCommands.DuplicateAreaTag
{
    public class DuplicateAreaTagCommandValidator : AbstractValidator<DuplicateAreaTagCommand>
    {
        public DuplicateAreaTagCommandValidator(ITagValidator tagValidator, IProjectValidator projectValidator)
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(command => command)
                .MustAsync((command, token) => NotBeAClosedProjectForTagAsync(command.TagId, token))
                .WithMessage(command => $"Project is closed! Tag={command.TagId}")
                .MustAsync((command, token) => BeAnExistingSourceTagAsync(command.TagId, token))
                .WithMessage(command => $"Tag doesn't exist! Tag={command.TagId}")
                .MustAsync((command, token) => NotBeAnExistingTagWithinProjectAsync(command.GetTagNo(), command.TagId, token))
                .WithMessage(command => $"Tag already exists in scope for project! Tag={command.GetTagNo()}")
                .MustAsync((command, token) => IsReadyToBeDuplicatedAsync(command.TagId, token))
                .WithMessage(command => $"Tag can not be duplicated! Tag={command.TagId}");

            async Task<bool> NotBeAClosedProjectForTagAsync(int tagId, CancellationToken token)
                => !await projectValidator.IsClosedForTagAsync(tagId, token);
        
            async Task<bool> BeAnExistingSourceTagAsync(int tagId, CancellationToken token)
                => await tagValidator.ExistsAsync(tagId, token);

            async Task<bool> NotBeAnExistingTagWithinProjectAsync(string tagNo, int tagId, CancellationToken token)
                => !await tagValidator.ExistsAsync(tagNo, tagId, token);

            async Task<bool> IsReadyToBeDuplicatedAsync(int tagId, CancellationToken token)
                => await tagValidator.IsReadyToBeDuplicatedAsync(tagId, token);
        }
    }
}
