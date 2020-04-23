using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.UpdateJourney
{
    public class UpdateJourneyCommand : IRequest<Result<Unit>>
    {
        public UpdateJourneyCommand(int journeyId, string title)
        {
            JourneyId = journeyId;
            Title = title;
        }
        public int JourneyId { get; set;}
        public string Title { get; set; }

    }
}
