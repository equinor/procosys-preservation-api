using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.JourneyValidators;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.VoidJourney
{
    public class VoidJourneyCommandValidator : AbstractValidator<VoidJourneyCommand>
    {
        public VoidJourneyCommandValidator(IJourneyValidator journeyValidator)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(command => command)
                .MustAsync((command, token) => BeAnExistingJourneyAsync(command.JourneyId, token))
                .WithMessage(command => $"Journey doesn't exist! Journey={command.JourneyId}")
                .MustAsync((command, token) => NotBeAVoidedJourneyAsync(command.JourneyId, token))
                .WithMessage(command => $"Journey is already voided! Journey={command.JourneyId}");

            async Task<bool> BeAnExistingJourneyAsync(int journeyId, CancellationToken token)
                => await journeyValidator.ExistsAsync(journeyId, token);
            async Task<bool> NotBeAVoidedJourneyAsync(int journeyId, CancellationToken token)
                => !await journeyValidator.IsVoidedAsync(journeyId, token);
        }
    }
}
