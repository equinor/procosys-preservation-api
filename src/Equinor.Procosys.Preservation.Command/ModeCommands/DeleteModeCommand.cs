using MediatR;

namespace Equinor.Procosys.Preservation.Command.ModeCommands
{
    public class DeleteModeCommand : IRequest<Unit>
    {
        public DeleteModeCommand(int modeId)
        {
            ModeId = modeId;
        }

        public int ModeId { get; }
    }
}
