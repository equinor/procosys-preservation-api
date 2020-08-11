using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
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
            AutoTransferMethod autoTransferMethod)
        {
            JourneyId = journeyId;
            Title = title;
            ModeId = modeId;
            ResponsibleCode = responsibleCode;
            AutoTransferMethod = autoTransferMethod;
        }

        public int JourneyId { get; }
        public string Title { get; }
        public int ModeId { get; }
        public string ResponsibleCode { get; }
        public AutoTransferMethod AutoTransferMethod { get; }
    }
}
