using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.StepValidators;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.UnvoidStep
{
    public class UnvoidStepCommandValidator : AbstractValidator<UnvoidStepCommand>
    {
        public UnvoidStepCommandValidator(IStepValidator stepValidator)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(command => command)
                .MustAsync((command, token) => BeAnExistingStepAsync(command.StepId, token))
                .WithMessage(command => $"Step does not exist! Step={command.StepId}")
                .MustAsync((command, token) => BeAVoidedStepAsync(command.StepId, token))
                .WithMessage(command => $"Step is not voided! Step={command.StepId}");

            async Task<bool> BeAnExistingStepAsync(int stepId, CancellationToken token)
                => await stepValidator.ExistsAsync(stepId, token);
            async Task<bool> BeAVoidedStepAsync(int stepId, CancellationToken token)
                => await stepValidator.IsVoidedAsync(stepId, token);
        }
    }
}
