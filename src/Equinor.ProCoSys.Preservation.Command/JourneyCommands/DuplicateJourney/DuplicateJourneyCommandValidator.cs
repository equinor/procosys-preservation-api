﻿using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Validators.JourneyValidators;
using FluentValidation;

namespace Equinor.ProCoSys.Preservation.Command.JourneyCommands.DuplicateJourney
{
    public class DuplicateJourneyCommandValidator : AbstractValidator<DuplicateJourneyCommand>
    {
        public DuplicateJourneyCommandValidator(IJourneyValidator journeyValidator)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(command => command)
                .MustAsync((command, token) => BeAnExistingJourneyAsync(command.JourneyId, token))
                .WithMessage(command => $"Journey doesn't exist! Journey={command.JourneyId}")
                .MustAsync((command, token) => HaveUniqueJourneyTitleForDuplicateAsync(command.JourneyId, token))
                .WithMessage(_ => "Journey with title for the copy already exists!");

            async Task<bool> BeAnExistingJourneyAsync(int journeyId, CancellationToken token)
                => await journeyValidator.ExistsAsync(journeyId, token);
            async Task<bool> HaveUniqueJourneyTitleForDuplicateAsync(int journeyId, CancellationToken token)
                => !await journeyValidator.ExistsWithDuplicateTitleAsync(journeyId, token);
        }
    }
}
