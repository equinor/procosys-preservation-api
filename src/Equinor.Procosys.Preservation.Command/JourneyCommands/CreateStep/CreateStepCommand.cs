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
            string responsibleCode)
        {
            JourneyId = journeyId;
            Title = title;
            ModeId = modeId;
            ResponsibleCode = responsibleCode;
        }

        public int JourneyId { get; }
        public string Title { get; }
        public int ModeId { get; }
        public string ResponsibleCode { get; }
    }
}
