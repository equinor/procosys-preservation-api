using System.Linq;
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
                .MustAsync((command, token) => BeAnExistingStepAAsync(command.Steps.First().Id, token))
                .WithMessage(command => $"StepA does not exist! StepA={command.Steps.First().Id}")
                .MustAsync((command, token) => BeAnExistingStepBAsync(command.Steps.Skip(1).First().Id, token))
                .WithMessage(command => $"StepB does not exist! StepB={command.Steps.Skip(1).First().Id}")
                .MustAsync((command, token) => BeAdjacentStepsInAJourneyAsync(command.JourneyId, command.Steps.First().Id, command.Steps.Skip(1).First().Id, token))
                .WithMessage(command => $"StepA and StepB are not adjacent! StepA={command.Steps.First().Id}, StepB={command.Steps.Skip(1).First().Id}");

            async Task<bool> BeAnExistingJourneyAsync(int journeyId, CancellationToken token)
                => await journeyValidator.ExistsAsync(journeyId, token);
            async Task<bool> BeAnExistingStepAAsync(int stepId, CancellationToken token)
                => await stepValidator.ExistsAsync(stepId, token);
            async Task<bool> BeAnExistingStepBAsync(int stepId, CancellationToken token)
                => await stepValidator.ExistsAsync(stepId, token);
            async Task<bool> BeAdjacentStepsInAJourneyAsync(int journeyId, int stepAId, int stepBId, CancellationToken token)
                => await journeyValidator.AreAdjacentStepsInAJourneyAsync(journeyId, stepAId, stepBId, token);
        }
    }
}
