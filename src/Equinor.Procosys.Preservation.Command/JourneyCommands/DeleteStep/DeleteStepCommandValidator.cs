using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators;
using Equinor.Procosys.Preservation.Command.Validators.JourneyValidators;
using Equinor.Procosys.Preservation.Command.Validators.StepValidators;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.DeleteStep
{
    public class DeleteStepCommandValidator : AbstractValidator<DeleteStepCommand>
    {
        public DeleteStepCommandValidator(
            IJourneyValidator journeyValidator,
            IStepValidator stepValidator,
            IRowVersionValidator rowVersionValidator)
        {
            CascadeMode = CascadeMode.Stop;
            
            RuleFor(command => command)
                .MustAsync((command, token) => BeAnExistingJourneyAsync(command.JourneyId, token))
                .WithMessage(command => $"Journey doesn't exist! Journey={command.JourneyId}")
                .MustAsync((command, token) => BeAnExistingStepInJourneyAsync(command.JourneyId, command.StepId, token))
                .WithMessage(command => $"Step doesn't exists within given journey! Step={command.StepId}")
                .MustAsync((command, token) => BeAVoidedStepAsync(command.StepId, token))
                .WithMessage(command => $"Step is not voided! Step={command.StepId}")
                .MustAsync((command, token) => JourneyForStepNotBeUsedAsync(command.JourneyId, token))
                .WithMessage(command => $"No steps can be deleted from journey when preservation tags exists in journey! Journey={command.JourneyId}")
                .Must(command => HaveAValidRowVersion(command.RowVersion))
                .WithMessage(command => $"Not a valid row version! Row version={command.RowVersion}");

            async Task<bool> BeAnExistingJourneyAsync(int journeyId, CancellationToken token)
                => await journeyValidator.ExistsAsync(journeyId, token);
            
            async Task<bool> BeAnExistingStepInJourneyAsync(int journeyId, int stepId, CancellationToken token)
                => await journeyValidator.StepExistsAsync(journeyId, stepId, token);
            
            async Task<bool> BeAVoidedStepAsync(int stepId, CancellationToken token)
                => await stepValidator.IsVoidedAsync(stepId, token);
            
            async Task<bool> JourneyForStepNotBeUsedAsync(int journeyId, CancellationToken token)
                => !await journeyValidator.IsAnyStepInJourneyInUseAsync(journeyId, token);

            bool HaveAValidRowVersion(string rowVersion)
                => rowVersionValidator.IsValid(rowVersion);
        }
    }
}
