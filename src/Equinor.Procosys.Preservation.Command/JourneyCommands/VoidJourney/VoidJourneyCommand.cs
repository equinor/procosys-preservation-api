using System;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.VoidJourney
{
    public class VoidJourneyCommand : IRequest<Result<string>>
    {
        public VoidJourneyCommand(int journeyId, string rowVersion, Guid currentUserOid)
        {
            JourneyId = journeyId;
            RowVersion = rowVersion;
            CurrentUserOid = currentUserOid;
        }

        public int JourneyId { get; }
        public string RowVersion { get; }
        public Guid CurrentUserOid { get; }
    }
}
