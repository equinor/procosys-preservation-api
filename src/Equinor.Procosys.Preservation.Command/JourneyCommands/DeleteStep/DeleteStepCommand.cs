using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.JourneyCommands.DeleteStep
{
    public class DeleteStepCommand : IRequest<Result<Unit>>
    {
        public DeleteStepCommand(int journeyId, int stepId, string rowVersion)
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
