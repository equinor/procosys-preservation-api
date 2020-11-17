using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.VoidStep
{
    public class VoidStepCommand : IRequest<Result<string>>
    {
        public VoidStepCommand(int journeyId, int stepId, string rowVersion)
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
