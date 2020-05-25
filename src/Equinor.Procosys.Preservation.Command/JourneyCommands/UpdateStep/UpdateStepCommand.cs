using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.UpdateStep
{
    public class UpdateStepCommand : IRequest<Result<string>>
    {
        public UpdateStepCommand(int journeyId, int stepId, int modeId, int responsibleId, string title, string rowVersion)
        {
            JourneyId = journeyId;
            StepId = stepId;
            ModeId = modeId;
            ResponsibleId = responsibleId;
            Title = title;
            RowVersion = rowVersion;
        }
        public int JourneyId { get; }
        public int StepId { get; }
        public int ModeId { get; }
        public int ResponsibleId { get; }
        public string Title { get; }
        public string RowVersion { get; }
    }
}
