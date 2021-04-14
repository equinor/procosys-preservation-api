using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.JourneyCommands.CreateJourney
{
    public class CreateJourneyCommand : IRequest<Result<int>>
    {
        public CreateJourneyCommand(string title) => Title = title;

        public string Title { get; }
    }
}
