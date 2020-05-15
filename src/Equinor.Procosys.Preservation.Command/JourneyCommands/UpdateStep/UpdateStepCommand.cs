using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.UpdateStep
{
    public class UpdateStepCommand : IRequest<Result<string>>
    {
        public UpdateStepCommand(int journeyId, int stepId, string title, string rowVersion)
        {
            JourneyId = journeyId;
            StepId = stepId;
            Title = title;
            RowVersion = rowVersion;
        }
        public int JourneyId { get; }
        public int StepId { get; }
        public string Title { get; }
        public string RowVersion { get; }
    }
}
