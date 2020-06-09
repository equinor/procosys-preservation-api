using System;
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
            Guid currentUserOid)
        {
            JourneyId = journeyId;
            Title = title;
            ModeId = modeId;
            ResponsibleCode = responsibleCode;
            CurrentUserOid = currentUserOid;
        }

        public int JourneyId { get; }
        public string Title { get; }
        public int ModeId { get; }
        public string ResponsibleCode { get; }
        public Guid CurrentUserOid { get; }
    }
}
