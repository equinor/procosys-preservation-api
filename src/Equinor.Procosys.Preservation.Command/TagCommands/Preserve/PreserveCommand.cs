using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.Preserve
{
    public class PreserveCommand : IRequest<Result<Unit>>, ITagCommandRequest
    {
        public PreserveCommand(int tagId) => TagId = tagId;

        public int TagId { get; }
    }
}
