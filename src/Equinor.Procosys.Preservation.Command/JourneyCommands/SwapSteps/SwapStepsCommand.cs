using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.SwapSteps
{
    public class SwapStepsCommand : IRequest<Result<IEnumerable<StepIdAndRowVersion>>>
    {
        public SwapStepsCommand(int journeyId, IEnumerable<StepIdAndRowVersion> steps)
        {
            JourneyId = journeyId;
            Steps = steps ?? new List<StepIdAndRowVersion>();
        }

        public int JourneyId { get; }

        public IEnumerable<StepIdAndRowVersion> Steps { get; }
    }
}
