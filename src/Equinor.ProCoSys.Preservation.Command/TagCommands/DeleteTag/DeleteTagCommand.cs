using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.TagCommands.DeleteTag
{
    public class DeleteTagCommand : IRequest<Result<Unit>>, ITagCommandRequest
    {
        public DeleteTagCommand(int tagId, string rowVersion)
        {
            TagId = tagId;
            RowVersion = rowVersion;
        }

        public int TagId { get; }
        public string RowVersion { get; }
    }
}
