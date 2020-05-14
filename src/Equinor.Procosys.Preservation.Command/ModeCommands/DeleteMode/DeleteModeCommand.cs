using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.ModeCommands.DeleteMode
{
    public class DeleteModeCommand : IRequest<Result<Unit>>
    {
        public DeleteModeCommand(int modeId, string rowVersion)
        {
            ModeId = modeId;
            RowVersion = rowVersion;
        }

        public int ModeId { get; }
        public string RowVersion { get; }
    }
}
