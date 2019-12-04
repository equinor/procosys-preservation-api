using MediatR;

namespace Equinor.Procosys.Preservation.Command.ModeCommands
{
    public class CreateModeCommand : IRequest<int>
    {
        public string Title { get; set; }
    }
}
