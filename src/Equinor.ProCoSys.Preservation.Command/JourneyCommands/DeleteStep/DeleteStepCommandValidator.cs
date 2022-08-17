using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Validators;
using Equinor.ProCoSys.Preservation.Command.Validators.JourneyValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.StepValidators;
using FluentValidation;

namespace Equinor.ProCoSys.Preservation.Command.JourneyCommands.DeleteStep
{
    public class DeleteStepCommandValidator : AbstractValidator<DeleteStepCommand>
    {
        public DeleteStepCommandValidator(
            IJourneyValidator journeyValidator,
            IStepValidator stepValidator,
            IRowVersionValidator rowVersionValidator)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            
            RuleFor(command => command)
                .MustAsync((command, token) => BeAnExistingStepAsync(command.JourneyId, command.StepId, token))
                .WithMessage(_ => "Journey and/or step doesn't exist!")
                .MustAsync((command, token) => BeAVoidedStepAsync(command.StepId, token))
                .WithMessage(command => $"Step is not voided! Step={command.StepId}")
                .MustAsync((command, token) => JourneyForStepNotBeUsedAsync(command.JourneyId, token))
                .WithMessage(command => $"No steps can be deleted from journey when preservation tags exists in journey! Journey={command.JourneyId}")
                .Must(command => HaveAValidRowVersion(command.RowVersion))
                .WithMessage(command => $"Not a valid row version! Row version={command.RowVersion}");
            
            async Task<bool> BeAnExistingStepAsync(int journeyId, int stepId, CancellationToken token)
                => await journeyValidator.ExistsStepAsync(journeyId, stepId, token);
            
            async Task<bool> BeAVoidedStepAsync(int stepId, CancellationToken token)
                => await stepValidator.IsVoidedAsync(stepId, token);
            
            async Task<bool> JourneyForStepNotBeUsedAsync(int journeyId, CancellationToken token)
                => !await journeyValidator.IsAnyStepInJourneyInUseAsync(journeyId, token);

            bool HaveAValidRowVersion(string rowVersion)
                => rowVersionValidator.IsValid(rowVersion);
        }
    }
}
