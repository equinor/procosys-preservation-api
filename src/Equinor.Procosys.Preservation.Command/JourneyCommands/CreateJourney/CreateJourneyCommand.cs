using System;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.CreateJourney
{
    public class CreateJourneyCommand : IRequest<Result<int>>
    {
        public CreateJourneyCommand(string plant, string title)
        {
            Plant = plant ?? throw new ArgumentNullException(nameof(plant));
            Title = title;
        }

        public string Plant { get; }
        public string Title { get; }
    }
}
