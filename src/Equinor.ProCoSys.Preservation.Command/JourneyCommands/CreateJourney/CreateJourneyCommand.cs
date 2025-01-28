using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.JourneyCommands.CreateJourney
{
    public class CreateJourneyCommand(string title, string projectName = null) : IRequest<Result<int>>
    {
        public string Title { get; } = title;
        public string ProjectName { get; } = projectName;
    }
}
