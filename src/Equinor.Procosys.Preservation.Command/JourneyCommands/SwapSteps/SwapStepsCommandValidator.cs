using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.JourneyValidators;
using Equinor.Procosys.Preservation.Command.Validators.StepValidators;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.SwapSteps
{
    public class SwapStepsCommandValidator : AbstractValidator<SwapStepsCommand>
    {
        public SwapStepsCommandValidator(
            IJourneyValidator journeyValidator,
            IStepValidator stepValidator)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(command => command)
                .MustAsync((command, token) => BeAnExistingJourneyAsync(command.JourneyId, token))
                .WithMessage(command => $"Journey does not exist! Journey={command.JourneyId}")
                .MustAsync((command, token) => BeAnExistingStepAsync(command.StepAId, token))
                .WithMessage(command => $"StepA does not exists! StepA={command.StepAId}")
                .MustAsync((command, token) => BeAnExistingStepAsync(command.StepBId, token))
                .WithMessage(command => $"StepB does not exists! StepB={command.StepBId}")
                .MustAsync((command, token) => BeAdjacentStepsAsync(command.JourneyId, command.StepAId, command.StepBId, token))
                .WithMessage(command => $"StepA and StepB are not adjacent! StepA={command.StepAId}, StepB={command.StepBId}");

            async Task<bool> BeAnExistingJourneyAsync(int journeyId, CancellationToken token)
                => await journeyValidator.ExistsAsync(journeyId, token);
            async Task<bool> BeAnExistingStepAsync(int stepId, CancellationToken token)
                => await stepValidator.ExistsAsync(stepId, token);
            async Task<bool> BeAdjacentStepsAsync(int journeyId, int stepAId, int stepBId, CancellationToken token)
                => !await stepValidator.AreAdjacentAsync(journeyId, stepAId, stepBId, token);
        }
    }
}
