using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.SetStep
{
    public class SetStepCommand : IRequest<Result<Unit>>
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
