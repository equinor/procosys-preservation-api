using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.JourneyCommands.UnvoidStep
{
    public class UnvoidStepCommand : IRequest<Result<string>>
    {
        public UnvoidStepCommand(int journeyId, int stepId, string rowVersion)
        {
            JourneyId = journeyId;
            StepId = stepId;
            RowVersion = rowVersion;
        }

        public int JourneyId { get; }
        public int StepId { get; }
        public string RowVersion { get; }
    }
}
