using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.StepValidators;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.UpdateStep
{
    public class UpdateStepCommandValidator : AbstractValidator<UpdateStepCommand>
    {
        public UpdateStepCommandValidator(
            IStepValidator stepValidator)
        {
            RuleFor(command => command)
                .MustAsync((command, token) => BeAnExistingStepAsync(command.StepId, token))
                .WithMessage(command => $"Step doesn't exists! Step={command.StepId}")
                .MustAsync((command, token) => HaveUniqueStepTitleInJourneyAsync(command.StepId, command.Title, token))
                .WithMessage(command => $"Another step with title already exists in journey! Step={command.Title}");
            
            async Task<bool> BeAnExistingStepAsync(int stepId, CancellationToken token)
                => !await stepValidator.ExistsAsync(stepId, token);
            async Task<bool> HaveUniqueStepTitleInJourneyAsync(int stepId, string stepTitle, CancellationToken token)
                => !await stepValidator.ExistsInJourneyAsync(stepId, stepTitle, token);
        }
    }
}
