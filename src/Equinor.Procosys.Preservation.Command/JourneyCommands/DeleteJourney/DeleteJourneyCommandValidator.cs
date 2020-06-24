﻿using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.JourneyValidators;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.DeleteJourney
{
    public class DeleteJourneyCommandValidator : AbstractValidator<DeleteJourneyCommand>
    {
        public DeleteJourneyCommandValidator(IJourneyValidator journeyValidator)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            
            RuleFor(command => command)
                .MustAsync((command, token) => BeAnExistingJourneyAsync(command.JourneyId, token))
                .WithMessage(command => $"Journey doesn't exists! Journey={command.JourneyId}")
                .MustAsync((command, token) => BeAVoidedJourneyAsync(command.JourneyId, token))
                .WithMessage(command => $"Journey is not voided! Journey={command.JourneyId}")
                .MustAsync((command, token) => NotBeUsedAsync(command.JourneyId, token))
                .WithMessage(command => $"Journey is used! Journey={command.JourneyId}");

            async Task<bool> BeAnExistingJourneyAsync(int journeyId, CancellationToken token)
                => await journeyValidator.ExistsAsync(journeyId, token);

            async Task<bool> BeAVoidedJourneyAsync(int journeyId, CancellationToken token)
                => await journeyValidator.IsVoidedAsync(journeyId, token);

            async Task<bool> NotBeUsedAsync(int journeyId, CancellationToken token)
                => !await journeyValidator.IsInUseAsync(journeyId, token);
        }
    }
}
