using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.JourneyValidators;
using Equinor.Procosys.Preservation.Command.Validators.StepValidators;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.UpdateStep
{
    public class UpdateStepCommandValidator : AbstractValidator<UpdateStepCommand>
    {
        public UpdateStepCommandValidator(
            IJourneyValidator journeyValidator,
            IStepValidator stepValidator,
            IStepRepository stepRepository)
        {
            RuleFor(command => command)
                .MustAsync((command, token) => BeAnExistingStepAsync(command.StepId, token))
                .WithMessage(command => $"Step doesn't exists! Step={command.StepId}")
                .MustAsync((command, token) => HaveUniqueStepTitleAsync(command.StepId, command.Title, token))
                .WithMessage(command => $"A step with title already exists in journey! Step={command.Title}");
            
            async Task<bool> BeAnExistingStepAsync(int stepId, CancellationToken token)
                => !await stepValidator.ExistsAsync(stepId, token);
            async Task<bool> HaveUniqueStepTitleAsync(int stepId, string stepTitle, CancellationToken token) => 
                await stepValidator.ExistsAsync(stepId, stepTitle, token);
        }
    }
}
