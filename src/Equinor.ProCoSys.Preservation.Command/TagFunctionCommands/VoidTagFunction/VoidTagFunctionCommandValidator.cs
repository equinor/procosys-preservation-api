using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Validators;
using Equinor.ProCoSys.Preservation.Command.Validators.TagFunctionValidators;
using FluentValidation;

namespace Equinor.ProCoSys.Preservation.Command.TagFunctionCommands.VoidTagFunction
{
    public class VoidTagFunctionCommandValidator : AbstractValidator<VoidTagFunctionCommand>
    {
        public VoidTagFunctionCommandValidator(
            ITagFunctionValidator tagFunctionValidator,
            IRowVersionValidator rowVersionValidator)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(command => command)
                .MustAsync((command, token) => BeAnExistingTagFunctionAsync(command.TagFunctionCode, command.RegisterCode, token))
                .WithMessage(command => $"Tag function doesn't exist! Tag function={command.RegisterCode}/{command.TagFunctionCode}")
                .MustAsync((command, token) => NotBeAVoidedTagFunctionAsync(command.TagFunctionCode, command.RegisterCode, token))
                .WithMessage(command => $"Tag function is already voided! Tag function={command.RegisterCode}/{command.TagFunctionCode}")
                .Must(command => HaveAValidRowVersion(command.RowVersion))
                .WithMessage(command => $"Not a valid row version! Row version={command.RowVersion}");

            async Task<bool> BeAnExistingTagFunctionAsync(string tagFunctionCode, string registerCode, CancellationToken token)
                => await tagFunctionValidator.ExistsAsync(tagFunctionCode, registerCode, token);
            async Task<bool> NotBeAVoidedTagFunctionAsync(string tagFunctionCode, string registerCode, CancellationToken token)
                => !await tagFunctionValidator.IsVoidedAsync(tagFunctionCode, registerCode, token);
            bool HaveAValidRowVersion(string rowVersion)
                => rowVersionValidator.IsValid(rowVersion);
        }
    }
}
