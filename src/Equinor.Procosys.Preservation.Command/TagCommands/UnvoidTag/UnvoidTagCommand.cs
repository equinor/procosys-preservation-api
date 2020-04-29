using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.UnvoidTag
{
    public class UnvoidTagCommand : IRequest<Result<Unit>>, ITagCommandRequest
    {
        public UnvoidTagCommand(
            int tagId)
        {
            TagId = tagId;
        }
        public int TagId { get; }
    }
}
