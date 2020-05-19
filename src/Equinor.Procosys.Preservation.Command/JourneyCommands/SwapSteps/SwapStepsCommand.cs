using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.SwapSteps
{
    public class SwapStepsCommand : IRequest<Result<string>>
    {
        public SwapStepsCommand(int journeyId, int stepAId, string rowVersionA, int stepBId, string rowVersionB)
        {
            JourneyId = journeyId;
            StepAId = stepAId;
            RowVersionA = rowVersionA;
            StepBId = stepBId;
            RowVersionB = rowVersionB;
        }
        public int JourneyId { get; }
        public int StepAId { get; }
        public int StepBId { get; }
        public string RowVersionA { get; }
        public string RowVersionB { get; }
    }
}
