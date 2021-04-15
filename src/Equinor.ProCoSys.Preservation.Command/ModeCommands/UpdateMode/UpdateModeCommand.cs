using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.ModeCommands.UpdateMode
{
    public class UpdateModeCommand : IRequest<Result<string>>
    {
        public UpdateModeCommand(int modeId, string title, bool forSupplier, string rowVersion)
        {
            ModeId = modeId;
            Title = title;
            ForSupplier = forSupplier;
            RowVersion = rowVersion;
        }
        public int ModeId { get; }
        public string Title { get; }
        public bool ForSupplier { get; }
        public string RowVersion { get; }
    }
}
