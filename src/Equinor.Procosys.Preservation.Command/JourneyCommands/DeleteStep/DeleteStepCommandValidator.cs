using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.JourneyValidators;
using Equinor.Procosys.Preservation.Command.Validators.StepValidators;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.DeleteStep
{
    public class DeleteStepCommandValidator : AbstractValidator<DeleteStepCommand>
    {
        public DeleteStepCommandValidator(IJourneyValidator journeyValidator, IStepValidator stepValidator)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            
            RuleFor(command => command)
                .MustAsync((command, token) => BeAnExistingJourneyAsync(command.JourneyId, token))
                .WithMessage(command => $"Journey doesn't exists! Journey={command.JourneyId}")
                .MustAsync((command, token) => BeAnExistingStepInJourneyAsync(command.JourneyId, command.StepId, token))
                .WithMessage(command => $"Step doesn't exists! Step={command.StepId}")
                .MustAsync((command, token) => BeAVoidedStepAsync(command.StepId, token))
                .WithMessage(command => $"Step is not voided! Step={command.StepId}")
                .MustAsync((command, token) => JourneyForStepNotBeUsedAsync(command.JourneyId, token))
                .WithMessage(command => $"Journey owning step is used and no steps can be deleted! Journey={command.JourneyId}");
            
            async Task<bool> BeAnExistingJourneyAsync(int journeyId, CancellationToken token)
                => await journeyValidator.ExistsAsync(journeyId, token);
            
            async Task<bool> BeAnExistingStepInJourneyAsync(int journeyId, int stepId, CancellationToken token)
                => await journeyValidator.StepExistsAsync(journeyId, stepId, token);
            
            async Task<bool> BeAVoidedStepAsync(int stepId, CancellationToken token)
                => await stepValidator.IsVoidedAsync(stepId, token);
            
            async Task<bool> JourneyForStepNotBeUsedAsync(int journeyId, CancellationToken token)
                => !await journeyValidator.IsInUseAsync(journeyId, token);
        }
    }
}
