using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.VoidJourney
{
    public class VoidJourneyCommand : IRequest<Result<Unit>>
    {
        public VoidJourneyCommand(int journeyId) => JourneyId = journeyId;
        public int JourneyId { get; }
    }
}
