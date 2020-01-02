using MediatR;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.CreateJourney
{
    public class CreateJourneyCommand : IRequest<int>
    {
        public CreateJourneyCommand(string title) => Title = title;

        public string Title { get; }
    }
}
