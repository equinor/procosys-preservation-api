using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.VoidTag
{
    public class VoidTagCommand : IRequest<Result<Unit>>, ITagCommandRequest
    {
        public VoidTagCommand(
            int tagId,
            bool isVoided)
        {
            TagId = tagId;
            IsVoided = isVoided;
        }
        public int TagId { get; }
        public bool IsVoided { get; }
    }
}
