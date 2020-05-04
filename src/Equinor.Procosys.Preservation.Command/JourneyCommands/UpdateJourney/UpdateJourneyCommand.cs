using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.UpdateJourney
{
    public class UpdateJourneyCommand : IRequest<Result<Unit>>
    {
        public UpdateJourneyCommand(int journeyId, string title, ulong rowVersion)
        {
            JourneyId = journeyId;
            Title = title;
            RowVersion = rowVersion;
        }
        public int JourneyId { get; }
        public string Title { get; }
        public ulong RowVersion { get; }
    }
}
