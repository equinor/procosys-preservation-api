using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.UpdateStep
{
    public class UpdateStepCommand : IRequest<Result<string>>
    {
        public UpdateStepCommand(int journeyId,
            int stepId,
            int modeId,
            string responsibleCode,
            string title,
            AutoTransferMethod autoTransferMethod,
            string rowVersion)
        {
            JourneyId = journeyId;
            StepId = stepId;
            ModeId = modeId;
            ResponsibleCode = responsibleCode;
            Title = title;
            AutoTransferMethod = autoTransferMethod;
            RowVersion = rowVersion;
        }

        public int JourneyId { get; }
        public int StepId { get; }
        public int ModeId { get; }
        public string ResponsibleCode { get; }
        public string Title { get; }
        public AutoTransferMethod AutoTransferMethod { get; }
        public string RowVersion { get; }
    }
}
