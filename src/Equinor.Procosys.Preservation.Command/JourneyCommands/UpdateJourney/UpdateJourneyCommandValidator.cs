using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.JourneyValidators;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.UpdateJourney
{
    public class UpdateJourneyCommandValidator : AbstractValidator<UpdateJourneyCommand>
    {
        public UpdateJourneyCommandValidator(IJourneyValidator journeyValidator)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(command => command)
                .MustAsync((command, token) => BeAnExistingJourneyAsync(command.JourneyId, token))
                .WithMessage(command => $"Journey doesn't exists! Journey={command.JourneyId}")
                .MustAsync((command, token) => HaveUniqueJourneyTitleAsync(command.JourneyId, command.Title, token))
                .WithMessage(command => $"Another journey with this title already exists! Journey={command.Title}")
                .MustAsync((command, token) => NotBeAVoidedJourneyAsync(command.JourneyId, token))
                .WithMessage(command => $"Journey is voided! Journey={command.JourneyId}");

            async Task<bool> BeAnExistingJourneyAsync(int journeyId, CancellationToken token)
                => await journeyValidator.ExistsAsync(journeyId, token);
            async Task<bool> HaveUniqueJourneyTitleAsync(int journeyId, string journeyTitle, CancellationToken token) =>
                !await journeyValidator.ExistsWithSameTitleInAnotherJourneyAsync(journeyId, journeyTitle, token);
            async Task<bool> NotBeAVoidedJourneyAsync(int journeyId, CancellationToken token)
                => !await journeyValidator.IsVoidedAsync(journeyId, token);
        }
    }
}
