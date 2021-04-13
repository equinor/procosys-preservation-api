using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.ModeCommands.UnvoidMode
{
    public class UnvoidModeCommand : IRequest<Result<string>>
    {
        public UnvoidModeCommand(int modeId, string rowVersion)
        {
            ModeId = modeId;
            RowVersion = rowVersion;
        }

        public int ModeId { get; }
        public string RowVersion { get; }
    }
}
