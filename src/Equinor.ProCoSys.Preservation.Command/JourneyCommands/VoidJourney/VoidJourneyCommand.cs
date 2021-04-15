using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.JourneyCommands.VoidJourney
{
    public class VoidJourneyCommand : IRequest<Result<string>>
    {
        public VoidJourneyCommand(int journeyId, string rowVersion)
        {
            JourneyId = journeyId;
            RowVersion = rowVersion;
        }

        public int JourneyId { get; }
        public string RowVersion { get; }
    }
}
