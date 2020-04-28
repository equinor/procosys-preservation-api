using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.ModeCommands.UpdateMode
{
    public class UpdateModeCommand : IRequest<Result<Unit>>
    {
        public UpdateModeCommand(int modeId, string title)
        {
            ModeId = modeId;
            Title = title;
        }
        public int ModeId { get; set; }
        public string Title { get; set; }
    }
}
