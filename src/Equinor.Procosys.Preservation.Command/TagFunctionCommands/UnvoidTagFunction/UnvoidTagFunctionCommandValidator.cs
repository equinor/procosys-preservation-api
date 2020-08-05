using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators;
using Equinor.Procosys.Preservation.Command.Validators.TagFunctionValidators;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.TagFunctionCommands.UnvoidTagFunction
{
    public class UnvoidTagFunctionCommandValidator : AbstractValidator<UnvoidTagFunctionCommand>
    {
        public UnvoidTagFunctionCommandValidator(
            ITagFunctionValidator tagFunctionValidator,
            IRowVersionValidator rowVersionValidator)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(command => command)
                .MustAsync((command, token) => BeAnExistingTagFunctionAsync(command.TagFunctionCode, token))
                .WithMessage(command => $"Tag function doesn't exist! TagFunction={command.TagFunctionCode}")
                .MustAsync((command, token) => BeAVoidedTagFunctionAsync(command.TagFunctionCode, token))
                .WithMessage(command => $"Tag function is not voided! TagFunction={command.TagFunctionCode}")
                .Must(command => HaveAValidRowVersion(command.RowVersion))
                .WithMessage(command => $"Not a valid RowVersion! RowVersion={command.RowVersion}");

            async Task<bool> BeAnExistingTagFunctionAsync(string tagFunctionCode, CancellationToken token)
                => await tagFunctionValidator.ExistsAsync(tagFunctionCode, token);
            async Task<bool> BeAVoidedTagFunctionAsync(string tagFunctionCode, CancellationToken token)
                => await tagFunctionValidator.IsVoidedAsync(tagFunctionCode, token);
            bool HaveAValidRowVersion(string rowVersion)
                => rowVersionValidator.IsValid(rowVersion);
        }
    }
}
