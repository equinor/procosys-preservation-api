using MediatR;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands
{
    public class CreateStepCommand : IRequest<Unit>
    {
        public CreateStepCommand(int journeyId, int modeId, int responsibleId)
        {
            JourneyId = journeyId;
            ResponsibleId = responsibleId;
            ModeId = modeId;
        }

        public int JourneyId { get; }
        public int ResponsibleId { get; }
        public int ModeId { get; }
    }
}
