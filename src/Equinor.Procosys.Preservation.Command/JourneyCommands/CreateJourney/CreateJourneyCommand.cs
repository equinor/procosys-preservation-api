using System;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.CreateJourney
{
    public class CreateJourneyCommand : IRequest<Result<int>>
    {
        public CreateJourneyCommand(string title, Guid currentUserOid)
        {
            Title = title;
            CurrentUserOid = currentUserOid;
        }

        public string Title { get; }
        public Guid CurrentUserOid { get; }
    }
}
