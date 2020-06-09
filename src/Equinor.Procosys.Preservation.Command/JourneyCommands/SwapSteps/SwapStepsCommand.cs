using System;
using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.SwapSteps
{
    public class SwapStepsCommand : IRequest<Result<IEnumerable<StepIdAndRowVersion>>>
    {
        public SwapStepsCommand(int journeyId, int stepAId, string stepARowVersion, int stepBId, string stepBRowVersion, Guid currentUserOid)
        {
            JourneyId = journeyId;
            StepAId = stepAId;
            StepARowVersion = stepARowVersion;
            StepBId = stepBId;
            StepBRowVersion = stepBRowVersion;
            CurrentUserOid = currentUserOid;
        }
        public int JourneyId { get; }
        public int StepAId { get; }
        public string StepARowVersion { get; }
        public int StepBId { get; }
        public string StepBRowVersion { get; }
        public Guid CurrentUserOid { get; }
    }
}
