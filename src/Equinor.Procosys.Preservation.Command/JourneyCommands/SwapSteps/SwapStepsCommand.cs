using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.SwapSteps
{
    public class SwapStepsCommand : IRequest<Result<IEnumerable<StepIdAndRowVersion>>>
    {
        public SwapStepsCommand(int journeyId, int stepAId, string stepARowVersion, int stepBId, string stepBRowVersion)
        {
            JourneyId = journeyId;
            StepAId = stepAId;
            StepARowVersion = stepARowVersion;
            StepBId = stepBId;
            StepBRowVersion = stepBRowVersion;
        }
        public int JourneyId { get; }
        public int StepAId { get; }
        public string StepARowVersion { get; }
        public int StepBId { get; }
        public string StepBRowVersion { get; }
    }
}
