using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.ModeValidators;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.ModeCommands.CreateMode
{
    public class CreateModeCommandValidator : AbstractValidator<CreateModeCommand>
    {
        public CreateModeCommandValidator(IModeValidator modeValidator)
        {

            RuleFor(command => command)
                .MustAsync((command, token) => HaveUniqueTitleAsync(command.Title, token))
                .WithMessage(command => $"Mode with title already exists! Mode={command.Title}")
                .MustAsync((command, token) => NotExistForSupplierAsync(token))
                .WithMessage(command => $"Another mode for supplier already exists! Mode={command.Title}")
                .When(command => command.ForSupplier, ApplyConditionTo.CurrentValidator);

            async Task<bool> HaveUniqueTitleAsync(string title, CancellationToken token) =>
                !await modeValidator.ExistsWithSameTitleAsync(title, token);
            async Task<bool> NotExistForSupplierAsync(CancellationToken token) =>
                !await modeValidator.ExistsModeForSupplierAsync(token);
        }
    }
}
