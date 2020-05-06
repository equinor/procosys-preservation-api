using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.ModeCommands.UpdateMode
{
    public class UpdateModeCommand : IRequest<Result<Unit>>
    {
        public UpdateModeCommand(int modeId, string title, string rowVersion)
        {
            ModeId = modeId;
            Title = title;
            RowVersion = rowVersion;
        }
        public int ModeId { get; }
        public string Title { get; }
        public string RowVersion { get; }
    }
}
