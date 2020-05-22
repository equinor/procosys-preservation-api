using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.UpdateStep
{
    public class UpdateStepCommand : IRequest<Result<string>>
    {
        public UpdateStepCommand(int journeyId, int stepId, int modeId, string responsibleCode, string title, string rowVersion)
        {
            JourneyId = journeyId;
            StepId = stepId;
            ModeId = modeId;
            ResponsibleCode = responsibleCode;
            Title = title;
            RowVersion = rowVersion;
        }
        public int JourneyId { get; }
        public int StepId { get; }
        public int ModeId { get; }
        public string ResponsibleCode { get; }
        public string Title { get; }
        public string RowVersion { get; }
    }
}
