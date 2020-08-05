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
            bool transferOnRfccSign,
            bool transferOnRfocSign,
            string rowVersion)
        {
            JourneyId = journeyId;
            StepId = stepId;
            ModeId = modeId;
            ResponsibleCode = responsibleCode;
            Title = title;
            TransferOnRfccSign = transferOnRfccSign;
            TransferOnRfocSign = transferOnRfocSign;
            RowVersion = rowVersion;
        }

        public int JourneyId { get; }
        public int StepId { get; }
        public int ModeId { get; }
        public string ResponsibleCode { get; }
        public string Title { get; }
        public bool TransferOnRfccSign { get; }
        public bool TransferOnRfocSign { get; }
        public string RowVersion { get; }
    }
}
