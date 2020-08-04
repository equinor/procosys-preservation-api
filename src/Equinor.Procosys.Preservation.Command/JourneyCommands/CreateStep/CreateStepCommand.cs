using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.CreateStep
{
    public class CreateStepCommand : IRequest<Result<Unit>>
    {
        public CreateStepCommand(
            int journeyId,
            string title,
            int modeId,
            string responsibleCode,
            bool transferOnRfccSign,
            bool transferOnRfocSign)
        {
            JourneyId = journeyId;
            Title = title;
            ModeId = modeId;
            ResponsibleCode = responsibleCode;
            TransferOnRfccSign = transferOnRfccSign;
            TransferOnRfocSign = transferOnRfocSign;
        }

        public int JourneyId { get; }
        public string Title { get; }
        public int ModeId { get; }
        public string ResponsibleCode { get; }
        public bool TransferOnRfccSign { get; }
        public bool TransferOnRfocSign { get; }
    }
}
