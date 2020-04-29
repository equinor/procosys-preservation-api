using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.VoidTag
{
    public class VoidTagCommand : IRequest<Result<Unit>>, ITagCommandRequest
    {
        public VoidTagCommand(
            int tagId)
        {
            TagId = tagId;
        }
        public int TagId { get; }
    }
}
