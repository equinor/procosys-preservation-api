using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators;
using Equinor.Procosys.Preservation.Command.Validators.JourneyValidators;
using Equinor.Procosys.Preservation.Command.Validators.StepValidators;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.UnvoidStep
{
    public class UnvoidStepCommandValidator : AbstractValidator<UnvoidStepCommand>
    {
        public UnvoidStepCommandValidator(
            IJourneyValidator journeyValidator,
            IStepValidator stepValidator,
            IRowVersionValidator rowVersionValidator)
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(command => command)
                .MustAsync((command, token) => BeAnExistingStepAsync(command.JourneyId, command.StepId, token))
                .WithMessage(command => "Journey and/or step doesn't exist!")
                .MustAsync((command, token) => BeAVoidedStepAsync(command.StepId, token))
                .WithMessage(command => $"Step is not voided! Step={command.StepId}")
                .Must(command => HaveAValidRowVersion(command.RowVersion))
                .WithMessage(command => $"Not a valid row version! Row version={command.RowVersion}");

            async Task<bool> BeAnExistingStepAsync(int journeyId, int stepId, CancellationToken token)
                => await journeyValidator.ExistsStepAsync(journeyId, stepId, token);
            async Task<bool> BeAVoidedStepAsync(int stepId, CancellationToken token)
                => await stepValidator.IsVoidedAsync(stepId, token);
            bool HaveAValidRowVersion(string rowVersion)
                => rowVersionValidator.IsValid(rowVersion);
        }
    }
}
