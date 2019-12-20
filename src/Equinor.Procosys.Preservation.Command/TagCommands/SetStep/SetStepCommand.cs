using MediatR;

namespace Equinor.Procosys.Preservation.Command.TagCommands.SetStep
{
    public class SetStepCommand : IRequest<Unit>
    {
        public SetStepCommand(int tagId, int journeyId, int stepId)
        {
            TagId = tagId;
            JourneyId = journeyId;
            StepId = stepId;
        }

        public int TagId { get; }
        public int JourneyId { get; }
        public int StepId { get; }
    }
}
