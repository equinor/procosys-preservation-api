using MediatR;

namespace Equinor.Procosys.Preservation.Command.ModeCommands.CreateMode
{
    public class CreateModeCommand : IRequest<int>
    {
        public string Title { get; set; }
    }
}
