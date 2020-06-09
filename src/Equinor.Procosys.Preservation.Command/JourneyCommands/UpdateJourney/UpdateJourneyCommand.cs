using System;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.UpdateJourney
{
    public class UpdateJourneyCommand : IRequest<Result<string>>
    {
        public UpdateJourneyCommand(int journeyId, string title, string rowVersion, Guid currentUserOid)
        {
            JourneyId = journeyId;
            Title = title;
            RowVersion = rowVersion;
            CurrentUserOid = currentUserOid;
        }
        public int JourneyId { get; }
        public string Title { get; }
        public string RowVersion { get; }
        public Guid CurrentUserOid { get; }
    }
}
