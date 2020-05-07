using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.UpdateJourney
{
    public class UpdateJourneyCommand : IRequest<Result<string>>
    {
        public UpdateJourneyCommand(int journeyId, string title, string rowVersion)
        {
            JourneyId = journeyId;
            Title = title;
            RowVersion = rowVersion;
        }
        public int JourneyId { get; }
        public string Title { get; }
        public string RowVersion { get; }
    }
}
