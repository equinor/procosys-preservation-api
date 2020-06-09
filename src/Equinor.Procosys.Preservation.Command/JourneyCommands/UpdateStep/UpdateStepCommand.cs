using System;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.UpdateStep
{
    public class UpdateStepCommand : IRequest<Result<string>>
    {
        public UpdateStepCommand(int journeyId, int stepId, int modeId, string responsibleCode, string title, string rowVersion, Guid currentUserOid)
        {
            JourneyId = journeyId;
            StepId = stepId;
            ModeId = modeId;
            ResponsibleCode = responsibleCode;
            Title = title;
            RowVersion = rowVersion;
            CurrentUserOid = currentUserOid;
        }

        public int JourneyId { get; }
        public int StepId { get; }
        public int ModeId { get; }
        public string ResponsibleCode { get; }
        public string Title { get; }
        public string RowVersion { get; }
        public Guid CurrentUserOid { get; }
    }
}
