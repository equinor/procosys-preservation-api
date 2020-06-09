using System;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.UnvoidJourney
{
    public class UnvoidJourneyCommand : IRequest<Result<string>>
    {
        public UnvoidJourneyCommand(int journeyId, string rowVersion, Guid currentUserOid)
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
