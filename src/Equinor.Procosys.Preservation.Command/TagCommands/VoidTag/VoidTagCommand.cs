using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.VoidTag
{
    public class VoidTagCommand : IRequest<Result<string>>, ITagCommandRequest
    {
        public VoidTagCommand(int tagId, string rowVersion)
        {
            TagId = tagId;
            RowVersion = rowVersion;
        }

        public int TagId { get; }
        public string RowVersion { get; }
    }
}
