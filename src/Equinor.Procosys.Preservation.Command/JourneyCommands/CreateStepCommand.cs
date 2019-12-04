using MediatR;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands
{
    public class CreateStepCommand : IRequest<Unit>
    {
        public CreateStepCommand(int journeyId, int modeId, int tagId, int responsibleId)
        {
            JourneyId = journeyId;
            TagId = tagId;
            ResponsibleId = responsibleId;
            ModeId = modeId;
        }

        public int JourneyId { get; }
        public int TagId { get; }
        public int ResponsibleId { get; }
        public int ModeId { get; }
    }
}
