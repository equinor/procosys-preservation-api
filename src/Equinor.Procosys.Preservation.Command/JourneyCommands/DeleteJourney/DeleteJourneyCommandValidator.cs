using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators;
using Equinor.Procosys.Preservation.Command.Validators.JourneyValidators;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.DeleteJourney
{
    public class DeleteJourneyCommandValidator : AbstractValidator<DeleteJourneyCommand>
    {
        public DeleteJourneyCommandValidator(
            IJourneyValidator journeyValidator,
            IRowVersionValidator rowVersionValidator)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            
            RuleFor(command => command)
                .MustAsync((command, token) => BeAnExistingJourneyAsync(command.JourneyId, token))
                .WithMessage(command => $"Journey doesn't exist! Journey={command.JourneyId}")
                .MustAsync((command, token) => BeAVoidedJourneyAsync(command.JourneyId, token))
                .WithMessage(command => $"Journey is not voided! Journey={command.JourneyId}")
                .MustAsync((command, token) => NotBeUsedAsync(command.JourneyId, token))
                .WithMessage(command => $"Journey is used! Journey={command.JourneyId}")
                .MustAsync((command, token) => HaveAValidRowVersion(command.RowVersion, token))
                .WithMessage(command => $"Not a valid RowVersion! RowVersion={command.RowVersion}");

            async Task<bool> BeAnExistingJourneyAsync(int journeyId, CancellationToken token)
                => await journeyValidator.ExistsAsync(journeyId, token);

            async Task<bool> BeAVoidedJourneyAsync(int journeyId, CancellationToken token)
                => await journeyValidator.IsVoidedAsync(journeyId, token);

            async Task<bool> NotBeUsedAsync(int journeyId, CancellationToken token)
                => !await journeyValidator.IsInUseAsync(journeyId, token);

            async Task<bool> HaveAValidRowVersion(string rowVersion, CancellationToken token)
                => await rowVersionValidator.IsValid(rowVersion, token);
        }
    }
}
