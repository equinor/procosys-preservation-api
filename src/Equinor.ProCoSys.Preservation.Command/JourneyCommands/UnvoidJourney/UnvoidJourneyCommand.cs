using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.JourneyCommands.UnvoidJourney
{
    public class UnvoidJourneyCommand : IRequest<Result<string>>
    {
        public UnvoidJourneyCommand(int journeyId, string rowVersion)
        {
            JourneyId = journeyId;
            RowVersion = rowVersion;
        }

        public int JourneyId { get; }
        public string RowVersion { get; }
    }
}
