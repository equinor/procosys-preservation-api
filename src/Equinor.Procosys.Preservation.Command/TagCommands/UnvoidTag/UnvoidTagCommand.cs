using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.UnvoidTag
{
    public class UnvoidTagCommand : IRequest<Result<string>>, ITagCommandRequest
    {
        public UnvoidTagCommand(int tagId, string rowVersion)
        {
            TagId = tagId;
            RowVersion = rowVersion;
        }

        public int TagId { get; }
        public string RowVersion { get; }
    }
}
