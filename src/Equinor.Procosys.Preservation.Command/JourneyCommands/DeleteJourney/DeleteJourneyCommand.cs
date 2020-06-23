using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.DeleteJourney
{
    public class DeleteJourneyCommand : IRequest<Result<Unit>>
    {
        public DeleteJourneyCommand(int journeyId, string rowVersion)
        {
            JourneyId = journeyId;
            RowVersion = rowVersion;
        }

        public int JourneyId { get; }
        public string RowVersion { get; }
    }
}
