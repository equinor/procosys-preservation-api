using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.ModeCommands.DeleteMode
{
    public class DeleteModeCommand : IRequest<Result<Unit>>
    {
        public DeleteModeCommand(int modeId) => ModeId = modeId;

        public int ModeId { get; }
    }
}
