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
                .MustAsync((command, token) => BeAnExistingStepAsync(command.JourneyId, command.StepAId, token))
                .WithMessage(command => "Journey and/or step doesn't exist!")
                .MustAsync((command, token) => BeAnExistingStepAsync(command.JourneyId, command.StepBId, token))
                .WithMessage(command => "Journey and/or step doesn't exist!")
                .MustAsync((command, token) => BeAdjacentStepsInAJourneyAsync(command.JourneyId, command.StepAId, command.StepBId, token))
                .WithMessage(command => $"Steps are not adjacent! Steps={command.StepAId} and {command.StepBId}")
                .MustAsync((command, token) => NotIncludeAnySupplierStep(command.StepAId, command.StepBId, token))
                .WithMessage(command => $"Supplier steps cannot be swapped! Steps={command.StepAId} and {command.StepBId}")
                .Must(command => HaveAValidRowVersion(command.StepARowVersion))
                .WithMessage(command => $"Not a valid row version! Row version{command.StepARowVersion}")
                .Must(command => HaveAValidRowVersion(command.StepBRowVersion))
                .WithMessage(command => $"Not a valid row version! Row version={command.StepBRowVersion}");
            
            async Task<bool> BeAnExistingStepAsync(int journeyId, int stepId, CancellationToken token)
                => await journeyValidator.ExistsStepAsync(journeyId, stepId, token);
            
            async Task<bool> BeAdjacentStepsInAJourneyAsync(int journeyId, int stepAId, int stepBId, CancellationToken token)
                => await journeyValidator.AreAdjacentStepsInAJourneyAsync(journeyId, stepAId, stepBId, token);
            
            async Task<bool> NotIncludeAnySupplierStep(int stepAId, int stepBId, CancellationToken token)
                => !await stepValidator.IsAnyStepForSupplierAsync(stepAId, stepBId, token);

            bool HaveAValidRowVersion(string rowVersion)
                => rowVersionValidator.IsValid(rowVersion);
        }
    }
}
