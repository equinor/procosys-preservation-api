﻿using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Validators;
using Equinor.ProCoSys.Preservation.Command.Validators.JourneyValidators;
using FluentValidation;

namespace Equinor.ProCoSys.Preservation.Command.JourneyCommands.DeleteJourney
{
    public class DeleteJourneyCommandValidator : AbstractValidator<DeleteJourneyCommand>
    {
        public DeleteJourneyCommandValidator(
            IJourneyValidator journeyValidator,
            IRowVersionValidator rowVersionValidator)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            
            RuleFor(command => command)
                .MustAsync((command, token) => BeAnExistingJourneyAsync(command.JourneyId, token))
                .WithMessage(command => $"Journey doesn't exist! Journey={command.JourneyId}")
                .MustAsync((command, token) => BeAVoidedJourneyAsync(command.JourneyId, token))
                .WithMessage(command => $"Journey is not voided! Journey={command.JourneyId}")
                .MustAsync((command, token) => NoStepHasTagAsync(command.JourneyId, token))
                .WithMessage(command => $"Journey can not be deleted when preservation tags exists in journey! Journey={command.JourneyId}")
                .Must(command => HaveAValidRowVersion(command.RowVersion))
                .WithMessage(command => $"Not a valid row version! Row version={command.RowVersion}");

            async Task<bool> BeAnExistingJourneyAsync(int journeyId, CancellationToken token)
                => await journeyValidator.ExistsAsync(journeyId, token);

            async Task<bool> BeAVoidedJourneyAsync(int journeyId, CancellationToken token)
                => await journeyValidator.IsVoidedAsync(journeyId, token);

            async Task<bool> NoStepHasTagAsync(int journeyId, CancellationToken token)
                => !await journeyValidator.HasAnyStepInJourneyATagAsync(journeyId, token);

            bool HaveAValidRowVersion(string rowVersion)
                => rowVersionValidator.IsValid(rowVersion);
        }
    }
}
