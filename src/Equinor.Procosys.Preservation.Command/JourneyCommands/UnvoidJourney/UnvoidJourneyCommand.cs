using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.UnvoidJourney
{
    public class UnvoidJourneyCommand : IRequest<Result<Unit>>
    {
        public UnvoidJourneyCommand(int journeyId) =>  JourneyId = journeyId;
        public int JourneyId { get; }
    }
}
