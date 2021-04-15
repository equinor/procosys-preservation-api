using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.ActionCommands.CloseAction
{
    public class CloseActionCommand : IRequest<Result<string>>, ITagCommandRequest
    {
        public CloseActionCommand(int tagId, int actionId, string rowVersion)
        {
            TagId = tagId;
            ActionId = actionId;
            RowVersion = rowVersion;
        }
        public int TagId { get; }
        public int ActionId { get; }
        public string RowVersion { get; }
    }
}
