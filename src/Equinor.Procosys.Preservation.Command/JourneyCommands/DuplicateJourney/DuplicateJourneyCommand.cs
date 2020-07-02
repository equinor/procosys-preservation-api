using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.DuplicateJourney
{
    public class DuplicateJourneyCommand : IRequest<Result<int>>
    {
        public DuplicateJourneyCommand(int journeyId) => JourneyId = journeyId;

        public int JourneyId { get; }
    }
}
