using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.ModeCommands.VoidMode
{
    public class VoidModeCommand : IRequest<Result<string>>
    {
        public VoidModeCommand(int modeId, string rowVersion)
        {
            ModeId = modeId;
            RowVersion = rowVersion;
        }

        public int ModeId { get; }
        public string RowVersion { get; }
    }
}
