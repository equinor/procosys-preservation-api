using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.UpdateStep
{
    public class UpdateStepCommand : IRequest<Result<string>>
    {
        public UpdateStepCommand(int stepId, string title, string rowVersion)
        {
            StepId = stepId;
            Title = title;
            RowVersion = rowVersion;
        }
        public int StepId { get; }
        public string Title { get; }
        public string RowVersion { get; }
    }
}
