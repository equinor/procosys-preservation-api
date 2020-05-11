using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.UpdateStep
{
    public class UpdateStepCommand : IRequest<Result<Unit>>
    {
        public UpdateStepCommand(int stepId, string title, string rowVersion)
        {
            StepId = stepId;
            Title = title;
            RowVersion = rowVersion;
        }
        public int StepId { get; set; }
        public string Title { get; set; }
        public string RowVersion { get; set; }
    }
}
