using System;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.CreateStep
{
    public class CreateStepCommand : IRequest<Result<Unit>>
    {
        public CreateStepCommand(string plant, int journeyId, string title, int modeId, int responsibleId)
        {
            Plant = plant ?? throw new ArgumentNullException(nameof(plant));
            JourneyId = journeyId;
            Title = title;
            ResponsibleId = responsibleId;
            ModeId = modeId;
        }

        public string Plant { get; }
        public int JourneyId { get; }
        public string Title { get; }
        public int ResponsibleId { get; }
        public int ModeId { get; }
    }
}
