using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators;
using Equinor.Procosys.Preservation.Command.Validators.JourneyValidators;
using Equinor.Procosys.Preservation.Command.Validators.StepValidators;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.SwapSteps
{
    public class SwapStepsCommandValidator : AbstractValidator<SwapStepsCommand>
    {
        public SwapStepsCommandValidator(
            IJourneyValidator journeyValidator,
            IStepValidator stepValidator,
            IRowVersionValidator rowVersionValidator)
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(command => command)
                .MustAsync((command, token) => BeAnExistingJourneyAsync(command.JourneyId, token))
                .WithMessage(command => $"Journey does not exist! Journey={command.JourneyId}")
                .MustAsync((command, token) => BeAnExistingStepInJourneyAsync(command.JourneyId, command.StepAId, token))
                .WithMessage(command => $"StepA does not exist! StepA={command.StepAId}")
                .MustAsync((command, token) => BeAnExistingStepInJourneyAsync(command.JourneyId, command.StepBId, token))
                .WithMessage(command => $"StepB does not exist! StepB={command.StepBId}")
                .MustAsync((command, token) => BeAdjacentStepsInAJourneyAsync(command.JourneyId, command.StepAId, command.StepBId, token))
                .WithMessage(command => $"StepA and StepB are not adjacent! StepA={command.StepAId}, StepB={command.StepBId}")
                .MustAsync((command, token) => NotIncludeAnySupplierStep(command.StepAId, command.StepBId, token))
                .WithMessage(command => $"Supplier steps cannot be swapped! StepA={command.StepAId}, StepB={command.StepBId}")
                .Must(command => HaveAValidRowVersion(command.StepARowVersion))
                .WithMessage(command => $"Not a valid RowVersion for Step A! RowVersion={command.StepARowVersion}")
                .Must(command => HaveAValidRowVersion(command.StepBRowVersion))
                .WithMessage(command => $"Not a valid RowVersion for Step B! RowVersion={command.StepBRowVersion}");

            async Task<bool> BeAnExistingJourneyAsync(int journeyId, CancellationToken token)
                => await journeyValidator.ExistsAsync(journeyId, token);
            
            async Task<bool> BeAnExistingStepInJourneyAsync(int journeyId, int stepId, CancellationToken token)
                => await journeyValidator.StepExistsAsync(journeyId, stepId, token);
            
            async Task<bool> BeAdjacentStepsInAJourneyAsync(int journeyId, int stepAId, int stepBId, CancellationToken token)
                => await journeyValidator.AreAdjacentStepsInAJourneyAsync(journeyId, stepAId, stepBId, token);
            
            async Task<bool> NotIncludeAnySupplierStep(int stepAId, int stepBId, CancellationToken token)
                => !await stepValidator.IsAnyStepForSupplier(stepAId, stepBId, token);

            bool HaveAValidRowVersion(string rowVersion)
                => rowVersionValidator.IsValid(rowVersion);
        }
    }
}
